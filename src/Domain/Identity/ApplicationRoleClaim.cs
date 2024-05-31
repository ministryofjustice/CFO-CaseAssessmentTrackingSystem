using Microsoft.AspNetCore.Identity;

namespace Cfo.Cats.Domain.Identity;

public class ApplicationRoleClaim : IdentityRoleClaim<int>
{
    public string? Description { get; set; }
    public string? Group { get; set; }
    public virtual ApplicationRole Role { get; set; } = default!;
}
