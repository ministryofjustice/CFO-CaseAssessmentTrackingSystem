namespace Cfo.Cats.Infrastructure.Configurations;

/// <summary>
/// Configuration for the HMPPS External API integration, secured via mutual TLS (client certificate)
/// plus an API key. <see cref="ClientCertBase64"/> and <see cref="ClientCertPassword"/> are optional so
/// the integration degrades to a plain (non-mTLS) HTTP client when they are not configured - e.g. in
/// local development or in environments where the client certificate secret has not been provisioned.
/// </summary>
public class HmppsExternalApiOptions
{
    public const string HmppsExternalApi = "HmppsExternalApi";

    /// <summary>
    /// HMPPS External API base URL (unversioned). Must end with a trailing slash. Include the API
    /// version segment on individual relative request paths, e.g. client.GetAsync("v1/status", ...).
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Base64-encoded PKCS#12 (.pfx) client certificate bundle used for mutual TLS.
    /// </summary>
    public string? ClientCertBase64 { get; set; }

    /// <summary>
    /// Password protecting <see cref="ClientCertBase64"/>. Optional.
    /// </summary>
    public string? ClientCertPassword { get; set; }

    /// <summary>
    /// API key sent via the X-API-KEY header. Optional.
    /// </summary>
    public string? ApiKey { get; set; }
}
