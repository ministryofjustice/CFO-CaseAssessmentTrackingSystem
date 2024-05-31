using Microsoft.AspNetCore.Identity;

namespace Cfo.Cats.Domain.Identity;

public class ApplicationUserRole : IdentityUserRole<int>
{
    public virtual ApplicationUser User { get; set; } = default!;
    public virtual ApplicationRole Role { get; set; } = default!;
}
