using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetReassessments
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<ReassessmentsDto>>
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<ReassessmentsDto>>
    {
        public async Task<Result<ReassessmentsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var baseQuery = context.ReassessmentPayments
                .Where(mi => mi.AssessmentCompleted >= request.StartDate &&
                             mi.AssessmentCompleted < request.EndDate.AddDays(1).Date);

            // Checks and applies filter based on UserId or TenantId else throws exception
            baseQuery = request switch
            {
                { UserId: var userId } when !string.IsNullOrWhiteSpace(userId)
                    => baseQuery.Where(mi => mi.SupportWorker == userId),

                { TenantId: var tenantId } when !string.IsNullOrWhiteSpace(tenantId)
                    => baseQuery.Where(mi => mi.TenantId.StartsWith(tenantId)),

                _ => throw new ArgumentException("Invalid request: UserId or TenantId must be provided.")
            };

            var query = from mi in baseQuery
                        join l in context.Locations on mi.LocationId equals l.Id
                        group mi by l into grp
                        orderby grp.Key.Name, grp.Key.LocationType
                        select new Details(
                            grp.Key.Name,
                            grp.Key.LocationType,
                            grp.Count(mi => mi.EligibleForPayment),
                            grp.Count()
                        );

            var results = await query
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return new ReassessmentsDto(results);
        }
    }

    public record ReassessmentsDto
    {
        public ReassessmentsDto(Details[] details)
        {
            Details = details;
            
            Custody = details.Where(d => d.LocationType.IsCustody).Sum(d => d.TotalCount);
            CustodyPayable = details.Where(d => d.LocationType.IsCustody).Sum(d => d.Payable);

            Community = details.Where(d => d.LocationType.IsCommunity).Sum(d => d.TotalCount);
            CommunityPayable = details.Where(d => d.LocationType.IsCommunity).Sum(d => d.Payable);
        }

        public Details[] Details { get; }

        public int Custody { get; }
        public int CustodyPayable { get; }

        public int Community { get; }
        public int CommunityPayable { get; }

    }

    public record Details(string LocationName, LocationType LocationType, int Payable, int TotalCount);

}
