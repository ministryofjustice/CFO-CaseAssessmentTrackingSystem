using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Application.Features.Announcements.DTOs;

namespace Cfo.Cats.Application.Features.Announcements.Queries;

public static class GetAnnouncementTypes
{
    [RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
    public class Query : IQuery<Result<AnnouncementTypeDto[]>> { }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<AnnouncementTypeDto[]>>
    {
        public async Task<Result<AnnouncementTypeDto[]>> Handle(Query request, CancellationToken cancellationToken)
            => Result<AnnouncementTypeDto[]>.Success(await unitOfWork.DbContext.AnnouncementTypes.AsNoTracking()
                .OrderBy(x => x.Name).Select(x => new AnnouncementTypeDto(x.Id, x.Name)).ToArrayAsync(cancellationToken));
    }
}
