using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Announcements.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Announcements;

namespace Cfo.Cats.Application.Features.Announcements.Queries;

public static class GetAnnouncementsForManagement
{
    [RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
    public class Query : IQuery<Result<AnnouncementDto[]>> { }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IUserService userService,
        IMapper mapper) : IQueryHandler<Query, Result<AnnouncementDto[]>>
    {
        public async Task<Result<AnnouncementDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedException("Sign in to manage announcements.");
            var user = AnnouncementAccess.GetUser(userService, userId);
            var isInternal = UserProfile.IsInternalUser(user.UserName);
            var query = unitOfWork.DbContext.Announcements.AsNoTracking().Where(a => a.DeletedAt == null);
            if (!isInternal)
            {
                query = query.Where(a => !a.Revisions.Any(r =>
                    (r.Id == a.PendingRevisionId || r.Id == a.PublishedRevisionId)
                    && r.Audience == AnnouncementAudience.InternalStaff));
            }
            var announcements = await query.ToArrayAsync(cancellationToken);
            return Result<AnnouncementDto[]>.Success(announcements
                .OrderByDescending(a => (a.PendingRevision ?? a.PublishedRevision)!.SubmittedAt)
                .Select(a => new AnnouncementDto
                {
                    Id = a.Id,
                    Version = a.Version,
                    CreatorId = a.CreatorId,
                    Published = mapper.Map<AnnouncementRevisionDto>(a.PublishedRevision),
                    Pending = mapper.Map<AnnouncementRevisionDto>(a.PendingRevision),
                    History = a.Revisions
                        .Where(r => isInternal || r.Audience == AnnouncementAudience.AllStaff)
                        .OrderBy(r => r.SubmittedAt).ThenBy(r => r.Id)
                        .Select(r => mapper.Map<AnnouncementRevisionDto>(r)).ToArray()
                }).ToArray());
        }
    }
}
