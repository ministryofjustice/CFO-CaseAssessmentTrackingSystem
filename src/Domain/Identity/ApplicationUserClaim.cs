namespace Cfo.Cats.Domain.Identity;

public class ApplicationUserClaim : IdentityUserClaim<string>
{
    public string? Description { get; set; }
    public virtual ApplicationUser User { get; set; } = default!;
}
