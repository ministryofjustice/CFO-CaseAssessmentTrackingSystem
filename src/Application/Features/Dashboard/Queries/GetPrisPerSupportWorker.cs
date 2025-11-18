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
            if (string.IsNullOrWhiteSpace(request.UserId) == false)
            {
                return await GetPrisByUserId(request, cancellationToken);
            }
            if (string.IsNullOrWhiteSpace(request.TenantId) == false)
            {
                return await GetPrisByTenantId(request, cancellationToken);
            }
            return Result<PrisPerSupportWorkerDto>.Failure("Invalid request: UserId or TenantId must be provided.");
        }

        private async Task<Result<PrisPerSupportWorkerDto>> GetPrisByUserId(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query = from pri in context.SupportAndReferralPayments
                        join l in context.Locations on pri.LocationId equals l.Id
                        where
                              pri.SupportWorker == request.UserId &&
                              pri.Approved >= request.StartDate
                              && pri.Approved <= request.EndDate
                        group pri by new
                        {
                            l.Name,
                            l.LocationType,
                            pri.SupportType,
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
        private async Task<Result<PrisPerSupportWorkerDto>> GetPrisByTenantId(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query = from pri in context.SupportAndReferralPayments
                        join l in context.Locations on pri.LocationId equals l.Id
                        where
                              pri.TenantId.StartsWith(request.TenantId!)
                              && pri.Approved >= request.StartDate
                              && pri.Approved <= request.EndDate
                        group pri by new
                        {
                            l.Name,
                            l.LocationType,
                            pri.SupportType,
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