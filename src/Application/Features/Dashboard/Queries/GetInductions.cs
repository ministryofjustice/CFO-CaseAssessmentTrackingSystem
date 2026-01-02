using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Humanizer;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;
public static class GetInductions
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<InductionsDto>>
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<InductionsDto>>
    {
        public async Task<Result<InductionsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var baseQuery = context.InductionPayments
                .Where(mi => mi.PaymentPeriod >= request.StartDate &&
                             mi.PaymentPeriod <= request.EndDate)
                .Where(mi => mi.EligibleForPayment);

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
                            LocationName: grp.Key.Name,
                            LocationType: grp.Key.LocationType,
                            Payable: grp.Count()
                        );

            var results = await query
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return new InductionsDto(results);
        }
    }

    public record InductionsDto
    {
        public InductionsDto(LocationDetail[] details)
        {
            Details = details;
            Custody = details.Where(d => d.LocationType.IsCustody).Sum(d => d.Payable);
            Community = details.Where(d => d.LocationType.IsCommunity).Sum(d => d.Payable);
        }

        public LocationDetail[] Details { get; }
        public int Custody { get; }
        public int Community { get; }
    }

    public record LocationDetail(string LocationName, LocationType LocationType, int Payable);

}