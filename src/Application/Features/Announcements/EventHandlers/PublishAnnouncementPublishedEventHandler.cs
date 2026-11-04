using Cfo.Cats.Application.Features.Announcements.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Announcements;

namespace Cfo.Cats.Application.Features.Announcements.EventHandlers;

public class PublishAnnouncementPublishedEventHandler(IUnitOfWork unitOfWork)
    : INotificationHandler<AnnouncementPublishedDomainEvent>
{
    public async Task Handle(AnnouncementPublishedDomainEvent notification, CancellationToken cancellationToken)
        => await unitOfWork.DbContext.InsertOutboxMessage(new AnnouncementPublishedIntegrationEvent(
            notification.AnnouncementId,
            notification.Revision.Id,
            notification.Revision.Title,
            notification.Revision.Audience,
            notification.DateOccurred.UtcDateTime));
}
