using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class SearchParticipants
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IQuery<Result<List<ParticipantSearchResultDto>>>
    {
        /// <summary>
        /// The text to search for. Matches against first name, last name,
        /// participant id and external identifiers.
        /// </summary>
        public string Keyword { get; set; } = string.Empty;

        /// <summary>
        /// The currently logged in user. Used to scope results to the
        /// user's tenant.
        /// </summary>
        public UserProfile? CurrentUser { get; set; }

        /// <summary>
        /// The maximum number of results to return.
        /// </summary>
        public int MaxResults { get; set; } = 10;
    }

    public class Handler(IUnitOfWork unitOfWork)
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

            var context = unitOfWork.DbContext;

            var query = from p in context.Participants
                        where p.Owner!.TenantId!.StartsWith(request.CurrentUser!.TenantId!)
                        select p;

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
                    CurrentLocation = p.CurrentLocation.Name
                })
                .ToListAsync(cancellationToken);

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

            RuleFor(r => r.CurrentUser)
                .NotNull();
        }
    }
}
