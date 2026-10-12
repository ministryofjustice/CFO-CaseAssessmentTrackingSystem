using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Announcements;

namespace Cfo.Cats.Application.Features.Announcements.Commands;

public static class ApproveAnnouncement
{
    [RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
    public class Command : ICommand<Result>
    {
        public Guid Id { get; set; }
        public Guid Version { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Version).NotEmpty();
        }
    }

    public class Handler(
        IAnnouncementRepository repository,
        ICurrentUserService currentUserService,
        IUserService userService) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedException("Sign in to approve announcements.");
            var announcement = await repository.GetByIdAsync(request.Id, cancellationToken);
            var user = AnnouncementAccess.GetUser(userService, userId);
            AnnouncementAccess.EnsureManagementAudience(user, announcement);
            if (user.IsInRole(RoleNames.SystemSupport)
                && announcement.PendingRevision?.SubmittedBy == userId)
            {
                announcement.AutoApprove(request.Version, userId, true);
            }
            else
            {
                announcement.Approve(request.Version, userId);
            }
            return Result.Success();
        }
    }
}
