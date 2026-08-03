namespace Cfo.Cats.Infrastructure.Configurations;

/// <summary>
/// Configuration for the DMS integration (candidate, offloc and Delius lookups), secured via
/// mutual TLS (client certificate) plus an API key. <see cref="ClientCertBase64"/> and
/// <see cref="ClientCertPassword"/> are optional so the integration degrades to a plain
/// (non-mTLS) HTTP client when they are not configured - e.g. in local development or in
/// environments where the client certificate secret has not been provisioned.
/// </summary>
public class DmsOptions
{
    public const string Dms = "DMS";

    /// <summary>
    /// DMS API base URL.
    /// </summary>
    public string ApplicationUrl { get; set; } = string.Empty;

    /// <summary>
    /// API key sent via the X-API-KEY header.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Base64-encoded PKCS#12 (.pfx) client certificate bundle used for mutual TLS.
    /// </summary>
    public string? ClientCertBase64 { get; set; }

    /// <summary>
    /// Password protecting <see cref="ClientCertBase64"/>. Optional.
    /// </summary>
    public string? ClientCertPassword { get; set; }
}
