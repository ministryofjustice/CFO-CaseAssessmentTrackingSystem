using System.ComponentModel.DataAnnotations.Schema;
using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Identity;

public class ApplicationUser : IdentityUser, IAuditable
{
    private readonly List<Note> _notes = new();

    public IReadOnlyCollection<Note> Notes => _notes.AsReadOnly();

    public ApplicationUser()
    {
        UserClaims = new HashSet<ApplicationUserClaim>();
        UserRoles = new HashSet<ApplicationUserRole>();
        Logins = new HashSet<ApplicationUserLogin>();
        Tokens = new HashSet<ApplicationUserToken>();
    }

    public string? DisplayName { get; set; }
    public string? ProviderId { get; set; }
    public string? TenantId { get; set; }
    public virtual Tenant? Tenant { get; set; }
    public string? TenantName { get; set; }
    [Column(TypeName = "text")]
    public string? ProfilePictureDataUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsLive { get; set; }
    public string? MemorablePlace { get; set; }
    public string? MemorableDate { get; set; }
    public string? RefreshToken { get; set; }
    public bool RequiresPasswordReset { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public virtual ICollection<ApplicationUserClaim> UserClaims { get; set; }
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
    public virtual ICollection<ApplicationUserToken> Tokens { get; set; }

    public string? SuperiorId { get; set; } = null;
    public ApplicationUser? Superior { get; set; } = null;
    public DateTime? Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }

    public ApplicationUser AddNote(Note note)
    {
        if(_notes.Contains(note) is false)
        {
            _notes.Add(note);
        }

        return this;
    }

}
