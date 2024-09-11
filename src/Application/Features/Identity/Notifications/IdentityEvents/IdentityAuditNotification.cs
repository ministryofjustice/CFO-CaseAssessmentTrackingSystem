namespace Cfo.Cats.Application.Features.Identity.Notifications.IdentityEvents;

public class IdentityAuditNotification : INotification
{

    private IdentityAuditNotification(IdentityActionType actionType, string ipAddress, string? userName = null, string? performedBy = null)
    {
        ActionType = actionType;
        UserName = userName;
        PerformedBy = performedBy ?? userName;
        IpAddress = ipAddress;
    }

    public IdentityActionType ActionType { get; private set; } 
    
    public string? UserName { get; private set; }

    public string? PerformedBy { get; private set; }
    
    public string IpAddress { get; private set; }

    public static IdentityAuditNotification UnknownUserNameNotification(string userName, string ipAddress)
        => new  (IdentityActionType.UnknownUser, ipAddress , userName);

    public static IdentityAuditNotification LoginFailedPassword(string userName, string ipAddress)
        => new  (IdentityActionType.IncorrectPasswordEntered,ipAddress, userName, null);
    public static IdentityAuditNotification LoginFailedTwoFactor(string userName, string ipAddress)
        => new  (IdentityActionType.IncorrectTwoFactorCodeEntered, ipAddress, userName, null);

    public static IdentityAuditNotification LoginSucceededNoTwoFactorRequired(string  userName, string ipAddress)
        => new(IdentityActionType.LoginPasswordOnly, ipAddress, userName);

    public static IdentityAuditNotification LoginSucceededTwoFactorRequired(string  userName, string ipAddress)
        => new(IdentityActionType.LoginWithTwoFactorCode, ipAddress, userName);

    public static IdentityAuditNotification UserLockedOut(string  userName, string ipAddress)
        => new(IdentityActionType.UserAccountLockedOut, ipAddress,userName);

    public static IdentityAuditNotification UserInactive(string userName, string ipAddress)
    => new(IdentityActionType.UserInactive, ipAddress, userName);

    public static IdentityAuditNotification PasswordReset(string userName, string ipAddress, string? performedBy = null)
        => new(IdentityActionType.PasswordReset,ipAddress , userName, performedBy);

    public static IdentityAuditNotification ActivateAccount(string userName,string ipAddress, string performedBy)
        => new(IdentityActionType.AccountActivated, ipAddress, userName, performedBy);

    public static IdentityAuditNotification DeactivateAccount(string userName, string ipAddress,  string performedBy)
        => new(IdentityActionType.AccountDeactivated, ipAddress, userName, performedBy);

}

