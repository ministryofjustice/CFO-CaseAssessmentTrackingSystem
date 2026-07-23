namespace Cfo.Cats.Infrastructure.Services.Identity;

public class AllowlistOptions
{
    /// <summary>
    /// Comma-delimited IPs. Ignored when <see cref="AllowedIPs"/> is configured.
    /// </summary>
    public string AllowedIPsFallback { get; set; } = string.Empty;

    /// <summary>
    /// List of allowed IPs (these bypass 2FA).
    /// </summary>
    public List<string> AllowedIPs { get; set; } = [];

    public IEnumerable<string> GetAllowedIPs()
    {
        if(AllowedIPs.Count > 0)
        {
            return AllowedIPs;
        }

        return AllowedIPsFallback.Split(',').Select(ip => ip.Trim()).ToList() ?? [];
    }
}