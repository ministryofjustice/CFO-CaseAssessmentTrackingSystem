namespace Cfo.Cats.Domain.Entities.Notifications;

public static class NotificationConstants
{
    /// The number of days after which a notification expires and can no longer be viewed.
    public const int ExpiryDays = 30;

    /// Notifications dated before this value have expired.
    public static DateTime ExpiryCutoff => DateTime.UtcNow.Date.AddDays(-ExpiryDays);
}
