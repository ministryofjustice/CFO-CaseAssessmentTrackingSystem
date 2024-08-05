using Microsoft.AspNetCore.Identity;

namespace Cfo.Cats.Domain.Identity;

public class ApplicationRole : IdentityRole
{
    public ApplicationRole()
    {
        RoleClaims = new HashSet<ApplicationRoleClaim>();
        UserRoles = new HashSet<ApplicationUserRole>();
    }

    public ApplicationRole(string roleName)
        : base(roleName)
    {
        RoleClaims = new HashSet<ApplicationRoleClaim>();
        UserRoles = new HashSet<ApplicationUserRole>();
    }

    public string? Description { get; set; }
    public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

    /// <summary>
    /// A higher ranking indicates more weighting in permissions.
    /// Used specifically for deciding who can and cannot assign roles to
    /// people.
    ///
    /// For example a QA Support Officer cannot create a user with SMT privileges 
    /// </summary>
    public int RoleRank { get; set; } = int.MaxValue;
}
