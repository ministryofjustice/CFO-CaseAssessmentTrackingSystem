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
        public required string UserId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }
    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<ApprovedActivitiesPerSupportWorkerDto>>
    {
        public async Task<Result<ApprovedActivitiesPerSupportWorkerDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query = from a in context.Activities 
                        join l in context.Locations on a.TookPlaceAtLocation.Id equals l.Id
                        where a.OwnerId == request.UserId
                              && a.CompletedOn >= request.StartDate
                              && a.CompletedOn <= request.EndDate
                              && a.Status == ActivityStatus.ApprovedStatus.Value
                        group a by new
                        {
                            l.Name,
                            l.LocationType,
                            a.Type
                        } into g
                        orderby g.Key.Name, g.Key.Type
                        select new Details(
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
        public ApprovedActivitiesPerSupportWorkerDto(Details[] details)
        {
            Details = details;
            Custody = details.Where(d => d.LocationType.IsCustody).Sum(d => d.Count);
            Community = details.Where(d => d.LocationType.IsCommunity).Sum(d => d.Count);

        }
        public Details[] Details { get; init; }
        public int Custody { get; }
        public int Community { get; }
    }

    public record Details(string Location, LocationType LocationType, ActivityType ActivityType, int Count);

}
