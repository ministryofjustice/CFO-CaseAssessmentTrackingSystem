namespace Cfo.Cats.Application.Features.Identity.Notifications.SendFactorCode;

public class SendFactorCodeNotificationHandler(
    ILogger<SendFactorCodeNotificationHandler> logger,
    ICommunicationsService communicationsService
) : INotificationHandler<SendFactorCodeNotification>
{
    public async Task Handle(SendFactorCodeNotification notification, CancellationToken cancellationToken)
    {
        await communicationsService.SendEmailCodeAsync(notification.Email, notification.AuthenticatorCode);
        logger.LogInformation("Verification Code email sent to {Email})", notification.Email);
    }
}