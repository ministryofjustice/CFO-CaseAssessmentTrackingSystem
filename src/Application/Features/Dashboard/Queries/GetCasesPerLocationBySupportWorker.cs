using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetCasesPerLocationBySupportWorker
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<CasesPerLocationSupportWorkerDto>>
    {
        public required string UserId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<CasesPerLocationSupportWorkerDto>>
    {
        public async Task<Result<CasesPerLocationSupportWorkerDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query =
                from p in context.Participants
                join l in context.Locations on p.CurrentLocation!.Id equals l.Id
                where p.OwnerId == request.UserId
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