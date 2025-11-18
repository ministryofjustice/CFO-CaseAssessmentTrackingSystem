using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetCasesPerLocationBySupportWorker
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<CasesPerLocationSupportWorkerDto>>
    {
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<CasesPerLocationSupportWorkerDto>>
    {
        public async Task<Result<CasesPerLocationSupportWorkerDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserId) == false)
            {
                return await GetCasesPerLocationByUserId(request.UserId, cancellationToken);
            }
            if (string.IsNullOrWhiteSpace(request.TenantId) == false)
            {
                return await GetCasesPerLocationByTenantId(request.TenantId, cancellationToken);
            }
            return Result<CasesPerLocationSupportWorkerDto>.Failure("Invalid request: UserId or TenantId must be provided.");          
        }

        private async Task<Result<CasesPerLocationSupportWorkerDto>> GetCasesPerLocationByUserId(string userId, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query =
                from p in context.Participants
                join l in context.Locations on p.CurrentLocation!.Id equals l.Id
                where p.OwnerId == userId
                      && p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value
                group p by new
                {
                    l.Name,
                    l.LocationType,
                    p.EnrolmentStatus
                } into grp
                select new LocationDetail
                (
                    grp.Key.Name,
                    grp.Key.LocationType,
                    grp.Key.EnrolmentStatus,
                    grp.Count()
                );

            var result = await query
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return new CasesPerLocationSupportWorkerDto(result);
        }
        private async Task<Result<CasesPerLocationSupportWorkerDto>> GetCasesPerLocationByTenantId(string tenantId, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query =
                from p in context.Participants
                join l in context.Locations on p.CurrentLocation!.Id equals l.Id
                join u in context.Users on p.OwnerId equals u.Id
                where u.TenantId!.StartsWith(tenantId)
                      && p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value
                group p by new
                {
                    l.Name,
                    l.LocationType,
                    p.EnrolmentStatus
                } into grp
                select new LocationDetail
                (
                    grp.Key.Name,
                    grp.Key.LocationType,
                    grp.Key.EnrolmentStatus,
                    grp.Count()
                );

            var result = await query
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return new CasesPerLocationSupportWorkerDto(result);
        }
    }

    public record CasesPerLocationSupportWorkerDto
    {
        public CasesPerLocationSupportWorkerDto(LocationDetail[] details)
        {
            Records = details;
            Custody = details.Where(d => d.LocationType.IsCustody).Sum(d => d.Count);
            Community = details.Where(d => d.LocationType.IsCommunity).Sum(d => d.Count);
        }
        public LocationDetail[] Records { get; }
        public int Custody { get; }
        public int Community { get; }
    }

    public record LocationDetail(string LocationName, LocationType LocationType, EnrolmentStatus Status, int Count);
}