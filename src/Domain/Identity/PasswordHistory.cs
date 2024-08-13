namespace Cfo.Cats.Domain.Identity;

public class PasswordHistory
{
    public int Id { get; set; }
    public string UserId { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
