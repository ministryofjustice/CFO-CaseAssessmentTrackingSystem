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

    /// <summary>
    /// Configuration for consuming HMPPS External API events from an SNS-fronted AWS SQS queue using
    /// temporary AWS credentials obtained from the HMPPS token endpoint. Reuses the mutual TLS client
    /// certificate and API key configured above.
    /// </summary>
    public HmppsExternalEventsOptions Events { get; set; } = new();
}

/// <summary>
/// Configuration for the HMPPS External Events SQS consumer. All values are optional so the consumer
/// can be left dormant (e.g. in local development) until the queue and token endpoint are provisioned.
/// </summary>
public class HmppsExternalEventsOptions
{
    /// <summary>
    /// Relative path (appended to <see cref="HmppsExternalApiOptions.BaseUrl"/>) of the HMPPS token
    /// endpoint that returns temporary AWS credentials for the queue. Authenticated with the same mTLS
    /// certificate and API key as the rest of the HMPPS External API.
    /// </summary>
    public string CredentialsEndpoint { get; set; } = "token";

    /// <summary>
    /// Fully-qualified URL of the SQS queue to poll.
    /// </summary>
    public string QueueUrl { get; set; } = string.Empty;

    /// <summary>
    /// AWS region the queue lives in, e.g. "eu-west-2".
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// Maximum number of messages to request per long-poll receive. AWS caps this at 10.
    /// </summary>
    public int MaxNumberOfMessages { get; set; } = 10;

    /// <summary>
    /// Long-poll wait time in seconds. AWS caps this at 20.
    /// </summary>
    public int WaitTimeSeconds { get; set; } = 20;

    /// <summary>
    /// How long before credential expiry the provider should proactively refresh. Defaults to 5 minutes.
    /// </summary>
    public TimeSpan RefreshWindow { get; set; } = TimeSpan.FromMinutes(5);
}
