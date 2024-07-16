namespace Cfo.Cats.Domain.Identity;

public class UserLogin : IdentityUserLogin<string>
{
    public virtual ApplicationUser User { get; set; } = default!;
}
