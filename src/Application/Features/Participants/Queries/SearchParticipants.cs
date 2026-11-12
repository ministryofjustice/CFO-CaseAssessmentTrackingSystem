using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Pipeline.ValidationSpecifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class SearchParticipants
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IQuery<Result<List<ParticipantSearchResultDto>>>
    {
        /// <summary>
        /// The text to search for. Matches against first name, last name,
        /// participant id and external identifiers. Searches every participant on the
        /// programme, regardless of whether the user can open the record.
        /// </summary>
        public string Keyword { get; set; } = string.Empty;

        /// <summary>
        /// The maximum number of results to return.
        /// </summary>
        public int MaxResults { get; set; } = 10;
    }

    public class Handler(IUnitOfWork unitOfWork, IEnumerable<AccessValidationSpecification> accessSpecifications)
        : IQueryHandler<Query, Result<List<ParticipantSearchResultDto>>>
    {
        public async Task<Result<List<ParticipantSearchResultDto>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var keyword = request.Keyword.Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Result<List<ParticipantSearchResultDto>>.Success([]);
            }

            var query = unitOfWork.DbContext.Participants.AsQueryable();

            if (keyword.Split(" ") is { Length: 2 } segments)
            {
                query = query.Where(p => p.FirstName.Contains(segments[0]) && p.LastName.Contains(segments[1]));
            }
            else
            {
                query = query.Where(p => p.FirstName.Contains(keyword)
                    || p.LastName!.Contains(keyword)
                    || p.Id.Contains(keyword)
                    || p.ExternalIdentifiers.Any(ei => ei.Value.Contains(keyword)));
            }

            var results = await query
                .AsNoTracking()
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Take(request.MaxResults)
                .Select(p => new ParticipantSearchResultDto
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    CurrentLocation = p.CurrentLocation.Name,
                    EnrolmentStatus = p.EnrolmentStatus,
                    IsUnassigned = p.OwnerId == null
                })
                .ToListAsync(cancellationToken);

            foreach (var result in results)
            {
                var grant = await ParticipantAccessEvaluator.EvaluateAsync(
                    accessSpecifications, result.Id, cancellationToken);
                result.HasAccess = grant is AccessGrant.Granted;
            }

            return Result<List<ParticipantSearchResultDto>>.Success(results);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(r => r.Keyword)
                .Matches(ValidationConstants.Keyword)
                .WithMessage(string.Format(ValidationConstants.KeywordMessage, "Search Keyword"));
        }
    }
}
