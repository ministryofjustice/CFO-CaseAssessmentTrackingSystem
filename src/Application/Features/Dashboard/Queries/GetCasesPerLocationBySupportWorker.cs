using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                select new
                {
                    g.Key.Name,
                    g.Key.EnrolmentStatus,
                    Count = g.Count()
                };

            var result = await query.AsNoTracking()
                .ToArrayAsync(cancellationToken);

            CasesPerLocationSupportWorkerDto dto = new CasesPerLocationSupportWorkerDto()
            {
                Records = result
                .OrderBy(r => r.Name)
                .ThenBy(r => r.EnrolmentStatus.Name)
                .Select(r => (r.Name, r.EnrolmentStatus.Name, r.Count))
                .ToArray()
            };

            return dto;

        }
    }
}

public record CasesPerLocationSupportWorkerDto
{
    public required (string Location, string Status, int Count)[] Records { get; init; }
}
