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
                where p.OwnerId == request.UserId
                      && p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value
                group p by new { p.CurrentLocation.Name, p.EnrolmentStatus } into g
                select new Details
                (
                    g.Key.Name,
                    g.Key.EnrolmentStatus,
                    g.Count()
                );

            var result = await query.AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return new CasesPerLocationSupportWorkerDto()
            {
                Records = result
            };

        }
    }
}

public record CasesPerLocationSupportWorkerDto
{
    public required Details[] Records { get; init; }
}

public record Details(string Location, EnrolmentStatus Status, int Count);
