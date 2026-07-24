using Cfo.Cats.Domain.Entities.Notifications;

namespace Cfo.Cats.Application.Features.Notifications.Specifications;

public sealed class NotificationAdvancedSpecification : Specification<Notification>
{
    public NotificationAdvancedSpecification(NotificationsAdvancedFilter filter) =>
        Query.Where(r => r.ReadDate == null, filter.ShowReadNotifications == false)
            .Where(n => n.OwnerId == filter.CurrentUser!.UserId)
            .Where(n => n.Heading.StartsWith("Activity"), filter.Type == NotificationType.Activities)
            .Where(n => n.Heading.StartsWith("Enrolment"), filter.Type == NotificationType.Enrolments)
            .Where(n => n.Heading == "Participant assigned", filter.Type == NotificationType.Assigned)
            .Where(n => n.Heading == "Participant unassigned", filter.Type == NotificationType.Unassigned)
            .Where(n => n.Heading == "PRI assigned", filter.Type == NotificationType.Pri)
            .Where(n => n.Heading.Contains(filter.Keyword!) || n.Details.Contains(filter.Keyword!),
                string.IsNullOrEmpty(filter.Keyword) == false);
}
