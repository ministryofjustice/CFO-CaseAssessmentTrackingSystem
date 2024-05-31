namespace Cfo.Cats.Server.UI.Services.Notifications;

public interface INotificationService
{
    Task<bool> AreNewNotificationsAvailable();
    Task MarkNotificationsAsRead();
    Task MarkNotificationsAsRead(string id);
    Task<NotificationMessage> GetMessageById(string id);
    Task<IDictionary<NotificationMessage, bool>> GetNotifications();
    Task AddNotification(NotificationMessage message);
}

public record NotificationAuthor(string DisplayName, string AvatarUlr);

public record NotificationMessage(
    string Id,
    string Title,
    string Except,
    string Category,
    DateTime PublishDate,
    string ImgUrl,
    IEnumerable<NotificationAuthor> Authors,
    Type ContentComponent
);
