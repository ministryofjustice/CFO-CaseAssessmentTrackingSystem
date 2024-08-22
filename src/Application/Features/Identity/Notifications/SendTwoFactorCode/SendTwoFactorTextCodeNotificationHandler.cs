namespace Cfo.Cats.Application.Features.Identity.Notifications.SendTwoFactorCode;

public class SendTwoFactorTextCodeNotificationHandler(ICommunicationsService communicationsService) 
    : INotificationHandler<SendTwoFactorTextCodeNotification>
{
    public async Task Handle(SendTwoFactorTextCodeNotification notification, CancellationToken cancellationToken)
    {
        await communicationsService.SendSmsCodeAsync(notification.MobileNumber, notification.AuthenticatorCode);
    }
}
