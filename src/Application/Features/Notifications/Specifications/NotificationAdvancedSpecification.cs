using Cfo.Cats.Domain.Entities.Notifications;

namespace Cfo.Cats.Application.Features.Notifications.Specifications;

public sealed class NotificationAdvancedSpecification : Specification<Notification>
{
    public NotificationAdvancedSpecification(NotificationsAdvancedFilter filter)
    {
        Query.Where(r => r.ReadDate == null, filter.IncludeReadNotifications == false)
            .Where(n => n.OwnerId == filter.CurrentUser!.UserId);
    }
}