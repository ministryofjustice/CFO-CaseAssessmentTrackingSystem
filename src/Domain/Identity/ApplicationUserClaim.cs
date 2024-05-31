using Microsoft.AspNetCore.Identity;

namespace Cfo.Cats.Domain.Identity;

public class ApplicationUserClaim : IdentityUserClaim<int>
{
    public string? Description { get; set; }
    public virtual ApplicationUser User { get; set; } = default!;
}
