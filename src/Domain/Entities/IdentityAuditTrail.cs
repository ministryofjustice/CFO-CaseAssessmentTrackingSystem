using System.ComponentModel.DataAnnotations.Schema;
using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Entities
{
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
        
        [NotMapped]
        public IReadOnlyCollection<DomainEvent> DomainEvents => new List<DomainEvent>().AsReadOnly();

        public void ClearDomainEvents() { }

        public static IdentityAuditTrail Create(string? userName, string? performedBy, IdentityActionType actionType )
        {
            return new IdentityAuditTrail()
            {
                UserName = userName,
                PerformedBy = performedBy,                
                DateTime = DateTime.Now,
                ActionType = actionType
            };
        }
    }

    public enum IdentityActionType 
    {
        /// <summary>
        /// An unknown user has tried to login
        /// </summary>
        UnknownUser, 

        /// <summary>
        /// The user has entered an incorrect password
        /// </summary>
        IncorrectPasswordEntered,
        /// <summary>
        /// The user has entered an incorrect two code
        /// </summary>
        IncorrectTwoFactorCodeEntered,
        CorrectPasswordEnteredTwoFactorRequired,
        LoginPasswordOnly,
        LoginWithTwoFactorCode,
        UserAccountLockedOut,
        PasswordReset,

        AccountActivated,
        AccountDeactivated
    }
}