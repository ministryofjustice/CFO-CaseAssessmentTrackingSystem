using Microsoft.AspNetCore.Identity;

namespace Cfo.Cats.Domain.Identity;

public class ApplicationUserLogin : IdentityUserLogin<int>
{
    public virtual ApplicationUser User { get; set; } = default!;
}
