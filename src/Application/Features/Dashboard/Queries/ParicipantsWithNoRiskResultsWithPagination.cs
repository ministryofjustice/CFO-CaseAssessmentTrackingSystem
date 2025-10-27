using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class ParticipantsWithNoRiskResultsWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : ParticipantsWithNoRiskResultsAdvancedFilter, IRequest<Result<PaginatedData<ParticipantsWithNoRiskDto>>>
    {
        public ParticipantsWithNoRiskResultsAdvancedSpecification Specification => new(this);
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<PaginatedData<ParticipantsWithNoRiskDto>>>
    {
        public async Task<Result<PaginatedData<ParticipantsWithNoRiskDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;

            var CFOTenantNames = new HashSet<string> { "CFO", "CFO Evolution" };

            var cutoffDate = DateTime.UtcNow.AddDays(-30);

#pragma warning disable CS8602
            var query = from p in db.Participants.ApplySpecification(request.Specification)                        
                        join owner in db.Users on p.OwnerId equals owner.Id
                        join u in db.Users on p.CreatedBy equals u.Id
                        join r in db.Risks on p.Id equals r.ParticipantId into riskGroup
                        from r in riskGroup.DefaultIfEmpty()
                        where r.Created == null
                        orderby p.Created ascending // oldest first

                        select new ParticipantsWithNoRiskDto
                        {
                            ParticipantId = p.Id,
                            ParticipantName = $"{p.FirstName} {p.LastName}",
                            EnrolmentStatus = p.EnrolmentStatus!,
                            CaseCreatedDate = p.Created!
                        };

  
                    var count = await query.CountAsync(cancellationToken);

            var results = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedData<ParticipantsWithNoRiskDto>(results, count, request.PageNumber, request.PageSize);
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(r => r.Keyword)
                    .Matches(ValidationConstants.Keyword)
                    .WithMessage(string.Format(ValidationConstants.KeywordMessage, "Search Keyword"));

                RuleFor(r => r.PageNumber)
                    .GreaterThan(0)
                    .WithMessage(string.Format(ValidationConstants.PositiveNumberMessage, "Page Number"));

                RuleFor(r => r.PageSize)
                    .GreaterThan(0)
                    .LessThanOrEqualTo(ValidationConstants.MaximumPageSize)
                    .WithMessage(ValidationConstants.MaximumPageSizeMessage);                
            }
        }
    }
}