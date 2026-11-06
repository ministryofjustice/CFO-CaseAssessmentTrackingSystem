namespace Cfo.Cats.Application.Common.Configuration;

public sealed class Feature
{
    public bool Enabled { get; set; }

    public string[] Tenants { get; set; } = [];

    /// <summary>
    /// Returns <c>true</c> when the feature is enabled and, if a tenant allow-list is configured,
    /// the supplied <paramref name="tenantId"/> exactly matches one of the allowed tenants.
    /// </summary>
    public bool IsEnabledForTenant(string? tenantId)
        => (Enabled, Tenants) switch
        {
            (false, _) => false,
            (true, []) => true,
            (true, { Length: > 0 } t ) when t.Contains(tenantId) => true,
            _ => false
        };
}
