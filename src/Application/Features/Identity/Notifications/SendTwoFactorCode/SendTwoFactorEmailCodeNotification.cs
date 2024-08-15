namespace Cfo.Cats.Application.Features.Identity.Notifications.SendTwoFactorCode;

public record SendTwoFactorEmailCodeNotification(string Email, string UserName, string AuthenticatorCode)
    : INotification;