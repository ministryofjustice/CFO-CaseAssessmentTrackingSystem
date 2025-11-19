using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Humanizer;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;
public static class GetInductionsPerSupportWorker
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<InductionsPerSupportWorkerDto>>
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<InductionsPerSupportWorkerDto>>
    {
        public async Task<Result<InductionsPerSupportWorkerDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var baseQuery = context.InductionPayments
                .Where(mi => mi.Approved >= request.StartDate &&
                             mi.Approved <= request.EndDate);

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
                        select new LocationDetail(
                            grp.Key.Name,
                            grp.Key.LocationType,
                            grp.Count(mi => mi.EligibleForPayment),
                            grp.Count()
                        );

            var results = await query
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return new InductionsPerSupportWorkerDto(results);
        }
    }

    public record InductionsPerSupportWorkerDto
    {
        public InductionsPerSupportWorkerDto(LocationDetail[] details)
        {
            Details = details;
            
            Custody = details.Where(d => d.LocationType.IsCustody).Sum(d => d.TotalCount);
            CustodyPayable = details.Where(d => d.LocationType.IsCustody).Sum(d => d.Payable);

            Community = details.Where(d => d.LocationType.IsCommunity).Sum(d => d.TotalCount);
            CommunityPayable = details.Where(d => d.LocationType.IsCommunity).Sum(d => d.Payable);

        }

        public LocationDetail[] Details { get; }

        public int Custody { get; }
        public int CustodyPayable { get; }

        public int Community { get; }
        public int CommunityPayable { get; }

    }

    public record LocationDetail(string LocationName, LocationType LocationType, int Payable, int TotalCount);

}