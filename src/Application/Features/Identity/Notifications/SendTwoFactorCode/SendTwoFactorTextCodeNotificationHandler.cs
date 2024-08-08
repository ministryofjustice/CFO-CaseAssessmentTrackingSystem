namespace Cfo.Cats.Application.Features.Identity.Notifications.SendTwoFactorCode;

public class SendTwoFactorTextCodeNotificationHandler(ICommunicationsService communicationsService, ILogger<SendTwoFactorTextCodeNotificationHandler> logger) 
    : INotificationHandler<SendTwoFactorTextCodeNotification>
{
    public async Task Handle(SendTwoFactorTextCodeNotification notification, CancellationToken cancellationToken)
    {
        await communicationsService.SendSmsCodeAsync(notification.MobileNumber, notification.AuthenticatorCode);
        logger.LogDebug("Verification Code email sent to {UserName})", notification.UserName);
    }
}
