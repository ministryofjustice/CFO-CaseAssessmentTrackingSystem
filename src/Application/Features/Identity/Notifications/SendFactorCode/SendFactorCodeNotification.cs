namespace Cfo.Cats.Application.Features.Identity.Notifications.SendFactorCode;

public record SendFactorCodeNotification(string Email, string UserName, string AuthenticatorCode)
    : INotification;