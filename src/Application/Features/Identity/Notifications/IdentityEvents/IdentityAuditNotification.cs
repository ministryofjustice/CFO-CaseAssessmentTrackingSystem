namespace Cfo.Cats.Application.Features.Identity.Notifications.IdentityEvents;

public class IdentityAuditNotification : INotification
{

    private IdentityAuditNotification(IdentityActionType actionType, string? userName = null, string? performedBy = null)
    {
        ActionType = actionType;
        UserName = userName;
        PerformedBy = performedBy ?? userName;
    }

    public IdentityActionType ActionType { get; private set; } 
    
    public string? UserName { get; private set; }

    public string? PerformedBy { get; private set; }

    public static IdentityAuditNotification UnknownUserNameNotification(string userName)
        => new  (IdentityActionType.UnknownUser, userName);

    public static IdentityAuditNotification LoginFailedPassword(string userName)
        => new  (IdentityActionType.IncorrectPasswordEntered, userName, null);
    public static IdentityAuditNotification LoginFailedTwoFactor(string userName)
        => new  (IdentityActionType.IncorrectTwoFactorCodeEntered, userName, null);

    public static IdentityAuditNotification LoginSucceededNoTwoFactorRequired(string  userName)
        => new(IdentityActionType.LoginPasswordOnly, userName);

    public static IdentityAuditNotification LoginSucceededTwoFactorRequired(string  userName)
        => new(IdentityActionType.LoginWithTwoFactorCode, userName);

    public static IdentityAuditNotification UserLockedOut(string  userName)
        => new(IdentityActionType.UserAccountLockedOut, userName);

    public static IdentityAuditNotification PasswordReset(string userName, string? performedBy = null)
        => new(IdentityActionType.PasswordReset, userName, performedBy);

}

