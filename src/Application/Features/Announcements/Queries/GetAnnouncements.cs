using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Announcements.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Announcements;

namespace Cfo.Cats.Application.Features.Announcements.Queries;

public static class GetAnnouncements
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IQuery<Result<AnnouncementDto[]>>
    {
        public AnnouncementAudience Audience { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Audience).IsInEnum();
        }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMapper mapper) : IQueryHandler<Query, Result<AnnouncementDto[]>>
    {
        public async Task<Result<AnnouncementDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            AnnouncementAccess.EnsureAudience(currentUserService.IsInternalUser.GetValueOrDefault(), request.Audience);
            var query = unitOfWork.DbContext.Announcements.AsNoTracking()
                .Where(a => a.DeletedAt == null
                    && a.Revisions.Any(r => r.Id == a.PublishedRevisionId && r.Audience == request.Audience));
            var announcements = await query.ToArrayAsync(cancellationToken);
            return Result<AnnouncementDto[]>.Success(announcements
                .OrderByDescending(a => a.PublishedRevision!.PublishedAt)
                .Select(a => new AnnouncementDto
                {
                    Id = a.Id,
                    Version = a.Version,
                    CreatorId = a.CreatorId,
                    Published = mapper.Map<AnnouncementRevisionDto>(a.PublishedRevision)
                }).ToArray());
        }
    }
}
