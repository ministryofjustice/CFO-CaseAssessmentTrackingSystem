using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Domain.Announcements;
using Cfo.Cats.Domain.Entities.Notifications;

namespace Cfo.Cats.Application.Features.Announcements.EventHandlers;

public class NotifyUsersOnAnnouncementPublished(
    IUnitOfWork unitOfWork,
    IUserService userService) : INotificationHandler<AnnouncementPublishedDomainEvent>
{
    public async Task Handle(AnnouncementPublishedDomainEvent notification, CancellationToken cancellationToken)
    {
        var revision = notification.Revision;
        var link = "/pages/workspace/my-area/announcements"
            + (revision.Audience == AnnouncementAudience.InternalStaff ? "?internal=true" : string.Empty);
        foreach (var user in userService.DataSource.Where(u => u.Status == UserStatus.Active))
        {
            if (revision.Audience == AnnouncementAudience.InternalStaff
                && !UserProfile.IsInternalUser(user.UserName))
            {
                continue;
            }
            var message = Notification.Create(
                "Announcement published",
                $"New Announcement: {revision.Title}",
                user.Id).SetLink(link);
            await unitOfWork.DbContext.Notifications.AddAsync(message, cancellationToken);
        }
    }
}
