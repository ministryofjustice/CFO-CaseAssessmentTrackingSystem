using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;
public static class GetApprovedActivitiesPerSupportWorker
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<ApprovedActivitiesPerSupportWorkerDto>>
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }
    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<ApprovedActivitiesPerSupportWorkerDto>>
    {
        public async Task<Result<ApprovedActivitiesPerSupportWorkerDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserId) == false)
            {
                return await GetApprovedActivitiesByUserId(request, cancellationToken);
            }
            if (string.IsNullOrWhiteSpace(request.TenantId) == false)
            {
                return await GetApprovedActivitiesByTenantId(request, cancellationToken);
            }
            return Result<ApprovedActivitiesPerSupportWorkerDto>.Failure("Invalid request: UserId or TenantId must be provided."); 
        }

        private async Task<Result<ApprovedActivitiesPerSupportWorkerDto>> GetApprovedActivitiesByUserId(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query = from a in context.Activities
                        join l in context.Locations on a.TookPlaceAtLocation.Id equals l.Id
                        where a.OwnerId == request.UserId
                              && a.CompletedOn >= request.StartDate
                              && a.CompletedOn < request.EndDate.AddDays(1).Date
                              && a.Status == ActivityStatus.ApprovedStatus.Value
                        group a by new
                        {
                            l.Name,
                            l.LocationType,
                            a.Type
                        } into g
                        orderby g.Key.Name, g.Key.Type
                        select new LocationDetail(
                            g.Key.Name,
                            g.Key.LocationType,
                            g.Key.Type,
                            g.Count()
                        );

            var results = await query
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return new ApprovedActivitiesPerSupportWorkerDto(results);
        }
        private async Task<Result<ApprovedActivitiesPerSupportWorkerDto>> GetApprovedActivitiesByTenantId(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query = from a in context.Activities
                        join l in context.Locations on a.TookPlaceAtLocation.Id equals l.Id
                        where a.TenantId.StartsWith(request.TenantId!)
                              && a.CompletedOn >= request.StartDate
                              && a.CompletedOn < request.EndDate.AddDays(1).Date
                              && a.Status == ActivityStatus.ApprovedStatus.Value
                        group a by new
                        {
                            l.Name,
                            l.LocationType,
                            a.Type
                        } into g
                        orderby g.Key.Name, g.Key.Type
                        select new LocationDetail(
                            g.Key.Name,
                            g.Key.LocationType,
                            g.Key.Type,
                            g.Count()
                        );

            var results = await query
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return new ApprovedActivitiesPerSupportWorkerDto(results);
        }

    }
    public record ApprovedActivitiesPerSupportWorkerDto
    {
        public ApprovedActivitiesPerSupportWorkerDto(LocationDetail[] details)
        {
            Details = details;
            Custody = details.Where(d => d.LocationType.IsCustody).Sum(d => d.Count);
            Community = details.Where(d => d.LocationType.IsCommunity).Sum(d => d.Count);

        }
        public LocationDetail[] Details { get; init; }
        public int Custody { get; }
        public int Community { get; }
    }

    public record LocationDetail(string LocationName, LocationType LocationType, ActivityType ActivityType, int Count);

}
