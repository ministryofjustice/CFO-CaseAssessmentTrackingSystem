using Cfo.Cats.Application.Features.Announcements.IntegrationEvents;
using Cfo.Cats.Domain.Announcements;
using Cfo.Cats.Domain.Entities.Notifications;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Announcements.IntegrationEventHandlers;

public class NotifyUsersOnAnnouncementPublishedConsumer(IUnitOfWork unitOfWork)
    : IHandleMessages<AnnouncementPublishedIntegrationEvent>
{
    public async Task Handle(AnnouncementPublishedIntegrationEvent message)
    {
        var recipients = unitOfWork.DbContext.Users.AsNoTracking()
            .Where(user => user.Status == UserStatus.Active);
        if (message.Audience == AnnouncementAudience.InternalStaff)
        {
            recipients = recipients.Where(user => user.Email != null
                && user.Email.ToLower().EndsWith("@justice.gov.uk"));
        }

        var userIds = await recipients.Select(user => user.Id).ToArrayAsync();
        var link = "/pages/workspace/my-area/announcements"
            + (message.Audience == AnnouncementAudience.InternalStaff ? "?internal=true" : string.Empty);
        foreach (var userId in userIds)
        {
            var notification = Notification.Create(
                "Announcement published", $"New Announcement: {message.Title}", userId).SetLink(link);
            await unitOfWork.DbContext.Notifications.AddAsync(notification);
        }
        await unitOfWork.SaveChangesAsync();
    }
}
