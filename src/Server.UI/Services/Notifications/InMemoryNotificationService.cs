using System.Security.Cryptography;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Cfo.Cats.Server.UI.Services.Notifications;

public class InMemoryNotificationService : INotificationService
{
    private const string LocalStorageKey = "__notficationTimestamp";
    private readonly ProtectedLocalStorage localStorageService;
    private readonly ILogger<InMemoryNotificationService> logger;

    private readonly List<NotificationMessage> messages;

    public InMemoryNotificationService(
        ProtectedLocalStorage localStorageService,
        ILogger<InMemoryNotificationService> logger
    )
    {
        this.localStorageService = localStorageService;
        this.logger = logger;
        messages = new List<NotificationMessage>();
    }

    public async Task<bool> AreNewNotificationsAvailable()
    {
        var timestamp = await GetLastReadTimestamp();
        var entriesFound = messages.Any(x => x.PublishDate > timestamp);

        return entriesFound;
    }

    public async Task MarkNotificationsAsRead()
    {
        await localStorageService.SetAsync(LocalStorageKey, DateTime.UtcNow.Date);
    }

    public async Task MarkNotificationsAsRead(string id)
    {
        var message = await GetMessageById(id);
        if (message == null)
        {
            return;
        }

        var timestamp = await localStorageService.GetAsync<DateTime>(LocalStorageKey);
        if (timestamp.Success)
        {
            await localStorageService.SetAsync(LocalStorageKey, message.PublishDate);
        }
    }

    public Task<NotificationMessage> GetMessageById(string id)
    {
        return Task.FromResult(messages.First(x => x.Id == id));
    }

    public async Task<IDictionary<NotificationMessage, bool>> GetNotifications()
    {
        var lastReadTimestamp = await GetLastReadTimestamp();
        var items = messages.ToDictionary(x => x, x => lastReadTimestamp > x.PublishDate);
        return items;
    }

    public Task AddNotification(NotificationMessage message)
    {
        messages.Add(message);
        return Task.CompletedTask;
    }

    private async Task<DateTime> GetLastReadTimestamp()
    {
        try
        {
            if ((await localStorageService.GetAsync<DateTime>(LocalStorageKey)).Success == false)
            {
                return DateTime.MinValue;
            }

            var timestamp = await localStorageService.GetAsync<DateTime>(LocalStorageKey);
            return timestamp.Value;
        }
        catch (CryptographicException)
        {
            await localStorageService.DeleteAsync(LocalStorageKey);
            return DateTime.MinValue;
        }
    }

    public void Preload()
    {
        messages.Add(
            new NotificationMessage(
                "readme",
                "Cats is ready",
                "We are paving the way for the future of CFO",
                "Announcement",
                new DateTime(2022, 01, 13),
                "",
                new[]
                {
                    new NotificationAuthor(
                        "Carl Sixsmith",
                        "https://avatars.githubusercontent.com/u/9332472?s=400&u=73c208bf07ba967d5407aae9068580539cfc80a2&v=4"
                    )
                },
                typeof(NotificationMessage)
            )
        );
    }
}
