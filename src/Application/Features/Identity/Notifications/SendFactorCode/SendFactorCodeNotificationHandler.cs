namespace Cfo.Cats.Application.Features.Identity.Notifications.SendFactorCode;

public class SendFactorCodeNotificationHandler : INotificationHandler<SendFactorCodeNotification>
{
    private readonly IStringLocalizer<SendFactorCodeNotificationHandler> localizer;
    private readonly ILogger<SendFactorCodeNotificationHandler> logger;
    private readonly IMailService mailService;

    public SendFactorCodeNotificationHandler(
        IStringLocalizer<SendFactorCodeNotificationHandler> localizer,
        ILogger<SendFactorCodeNotificationHandler> logger,
        IMailService mailService
    )
    {
        this.localizer = localizer;
        this.logger = logger;
        this.mailService = mailService;
    }

    public async Task Handle(
        SendFactorCodeNotification notification,
        CancellationToken cancellationToken
    )
    {
        var subject = localizer["Your Verification Code"];
        await mailService.SendAsync(notification.Email, subject, notification.AuthenticatorCode);
        logger.LogInformation("Verification Code email sent to {Email})", notification.Email);
    }
}