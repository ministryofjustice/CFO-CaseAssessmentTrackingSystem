using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetPrisPerSupportWorker
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<PrisPerSupportWorkerDto>>
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<PrisPerSupportWorkerDto>>
    {
        public async Task<Result<PrisPerSupportWorkerDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var baseQuery = context.SupportAndReferralPayments
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
                        group mi by new
                        {
                            l.Name,
                            l.LocationType,
                            mi.SupportType
                        } into g
                        orderby g.Key.Name, g.Key.SupportType
                        select new LocationDetail(
                            g.Key.Name,
                            g.Key.LocationType,
                            g.Key.SupportType,
                            g.Count(mi => mi.EligibleForPayment),
                            g.Count()
                        );

            var results = await query
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);
                
            return new PrisPerSupportWorkerDto(results);
        }

    }

    public record PrisPerSupportWorkerDto
    {
        public PrisPerSupportWorkerDto(LocationDetail[] details)
        {
            Details = details;

            Custody = details.Where(d => d.LocationType.IsCustody).Sum(d => d.TotalCount);
            Community = details.Where(d => d.LocationType.IsCommunity).Sum(d => d.TotalCount);
        }

        public LocationDetail[] Details { get; }
        public int Custody { get; }
        public int Community { get; }
    }

    public record LocationDetail(
        string LocationName,
        LocationType LocationType,
        string SupportType,
        int Payable,
        int TotalCount);
}