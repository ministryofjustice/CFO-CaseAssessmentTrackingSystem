using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class ParticipantsInLicenceEndPeriodResultsWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : ParticipantsInLicenceEndPeriodResultsAdvancedFilter, IRequest<Result<PaginatedData<ParticipantsInLicenceEndPeriodResultsSummaryDto>>>
    {
        public ParticipantsInLicenceEndPeriodResultsAdvancedSpecification Specification => new(this);
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<PaginatedData<ParticipantsInLicenceEndPeriodResultsSummaryDto>>>
    {
        public async Task<Result<PaginatedData<ParticipantsInLicenceEndPeriodResultsSummaryDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;

            var CFOTenantNames = new HashSet<string> { "CFO", "CFO Evolution" };

            var cutoffDate = DateTime.UtcNow.AddDays(-30);

#pragma warning disable CS8602
            var query = from p in db.Participants.ApplySpecification(request.Specification)
                        join owner in db.Users on p.OwnerId equals owner.Id
                        orderby p.DeactivatedInFeed ascending // oldest first
                        select new ParticipantsInLicenceEndPeriodResultsSummaryDto
                        {
                            Id = p.Id,
                            ParticipantName = $"{p.FirstName} {p.LastName}",
                            EnrolmentStatus = p.EnrolmentStatus!,
                            DeactivatedInFeed = p.DeactivatedInFeed,
                            TookPlaceAtLocationName = p.EnrolmentLocation.Name,
                            CaseWorkerDisplayName  = p.Owner.DisplayName!
                        };

#pragma warning restore CS8602

            var count = await query.CountAsync(cancellationToken);

            var results = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedData<ParticipantsInLicenceEndPeriodResultsSummaryDto>(results, count, request.PageNumber, request.PageSize);
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