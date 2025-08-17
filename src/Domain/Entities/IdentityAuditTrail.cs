using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Entities;

public class IdentityAuditTrail : IEntity<int>
{
    private IdentityAuditTrail()
    {}

    public int Id {get; private set;}

    /// <summary>
    /// The user the action was performed against.
    /// </summary>
    public string? UserName { get; private set; }

    /// <summary>
    /// The user the action was performed by. If null action was performed by a logged out user.
    /// </summary>
    public string? PerformedBy { get ; private set;}

    /// <summary>
    /// The date time of the action
    /// </summary>
    public DateTime DateTime {get; private set; }

    public IdentityActionType ActionType { get; private set; }
        
    public string? IpAddress { get; private set; }
        
    [NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents => new List<DomainEvent>().AsReadOnly();

    public void ClearDomainEvents() { }

    public static IdentityAuditTrail Create(string? userName, string? performedBy, IdentityActionType actionType, string ipAddress )
    {
        return new IdentityAuditTrail()
        {
            UserName = userName,
            PerformedBy = performedBy,                
            DateTime = DateTime.Now,
            ActionType = actionType,
            IpAddress = ipAddress
        };
    }
}

public enum IdentityActionType 
{
    /// <summary>
    /// An unknown user has tried to login
    /// </summary>
    [Description("Unknown User")]
    UnknownUser, 

    /// <summary>
    /// The user has entered an incorrect password
    /// </summary>
    [Description("Incorrect Password Entered")]
    IncorrectPasswordEntered,
    /// <summary>
    /// The user has entered an incorrect two code
    /// </summary>
    [Description("Incorrect Two Factor Code Entered")]
    IncorrectTwoFactorCodeEntered,
    [Description("Correct Password Entered Two Factor Required")]
    CorrectPasswordEnteredTwoFactorRequired,
    [Description("Login Password Only")]
    LoginPasswordOnly,
    [Description("Login With Two Factor Code")]
    LoginWithTwoFactorCode,
    [Description("User Account Locked Out")]
    UserAccountLockedOut,
    [Description("User Inactive")]
    UserInactive,
    [Description("Password Reset")]
    PasswordReset,
    [Description("Account Activated")]
    AccountActivated,
    [Description("Account Deactivated")]
    AccountDeactivated
}