namespace Cfo.Cats.Application.Features.Identity.Notifications.SendTwoFactorCode;

public class SendTwoFactorEmailCodeNotificationHandler(
    ILogger<SendTwoFactorEmailCodeNotificationHandler> logger,
    ICommunicationsService communicationsService
) : INotificationHandler<SendTwoFactorEmailCodeNotification>
{
    public async Task Handle(SendTwoFactorEmailCodeNotification notification, CancellationToken cancellationToken)
    {
        await communicationsService.SendEmailCodeAsync(notification.Email, notification.AuthenticatorCode);
    }
}
