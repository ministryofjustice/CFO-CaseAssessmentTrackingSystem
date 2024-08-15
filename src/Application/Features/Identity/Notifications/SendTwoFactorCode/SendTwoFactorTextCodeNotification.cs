namespace Cfo.Cats.Application.Features.Identity.Notifications.SendTwoFactorCode;

public record SendTwoFactorTextCodeNotification(string MobileNumber, string UserName, string AuthenticatorCode)
    : INotification;