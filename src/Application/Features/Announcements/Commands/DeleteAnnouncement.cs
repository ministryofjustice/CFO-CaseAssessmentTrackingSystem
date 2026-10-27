using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Announcements;

namespace Cfo.Cats.Application.Features.Announcements.Commands;

public static class DeleteAnnouncement
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
        ICurrentUserService currentUserService) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedException("Sign in to delete announcements.");
            var announcement = await repository.GetByIdAsync(request.Id, cancellationToken);
            if (!currentUserService.IsInternalUser.GetValueOrDefault()
                && (announcement.PendingRevision?.Audience == AnnouncementAudience.InternalStaff
                    || announcement.PublishedRevision?.Audience == AnnouncementAudience.InternalStaff))
            {
                throw new ForbiddenException("Only internal staff can access internal announcements.");
            }
            announcement.Delete(request.Version, userId);
            return Result.Success();
        }
    }
}
