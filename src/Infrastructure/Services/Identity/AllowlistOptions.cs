namespace Cfo.Cats.Infrastructure.Services.Identity;

public class AllowlistOptions
{
    public List<string> AllowedIPs { get; set; } = new();
}
