using System.Net.Http.Json;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Runtime;
using Cfo.Cats.Infrastructure.Configurations;

namespace Cfo.Cats.Infrastructure.Services.ExternalEvents;

/// <summary>
/// Obtains temporary AWS credentials from the HMPPS token endpoint (authenticated with the same
/// mutual TLS certificate and API key as the rest of the HMPPS External API), caches them, and
/// refreshes them shortly before they expire. Thread-safe: concurrent callers share a single refresh.
/// </summary>
public class HmppsAwsCredentialProvider(
    IHttpClientFactory httpClientFactory,
    IOptions<HmppsExternalApiOptions> options,
    ILogger<HmppsAwsCredentialProvider> logger) : IAwsCredentialProvider
{
    private readonly HmppsExternalEventsOptions _events = options.Value.Events;
    private readonly SemaphoreSlim _gate = new(1, 1);

    private SessionAWSCredentials? _cached;
    private DateTime _expiresAtUtc = DateTime.MinValue;

    public async Task<SessionAWSCredentials> GetCredentialsAsync(CancellationToken cancellationToken)
    {
        if (IsUsable())
        {
            return _cached!;
        }

        await _gate.WaitAsync(cancellationToken);
        try
        {
            // Re-check after acquiring the lock in case another caller already refreshed.
            if (IsUsable())
            {
                return _cached!;
            }

            await RefreshAsync(cancellationToken);
            return _cached!;
        }
        finally
        {
            _gate.Release();
        }
    }

    private bool IsUsable() =>
        _cached is not null && DateTime.UtcNow < _expiresAtUtc - _events.RefreshWindow;

    private async Task RefreshAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching temporary AWS credentials from the HMPPS token endpoint");

        var client = httpClientFactory.CreateClient(HmppsExternalEventsRegistration.TokenClientName);

        using var httpResponse = await client.PostAsync(_events.CredentialsEndpoint, content: null, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();

        var response = await httpResponse.Content.ReadFromJsonAsync<AwsCredentialsResponse>(
            cancellationToken);

        if (response is null
            || string.IsNullOrWhiteSpace(response.AccessKeyId)
            || string.IsNullOrWhiteSpace(response.SecretAccessKey)
            || string.IsNullOrWhiteSpace(response.SessionToken))
        {
            throw new InvalidOperationException("HMPPS token endpoint returned incomplete AWS credentials.");
        }

        _cached = new SessionAWSCredentials(
            response.AccessKeyId,
            response.SecretAccessKey,
            response.SessionToken);

        _expiresAtUtc = ParseExpiration(response.Expiration);

        logger.LogInformation("Obtained temporary AWS credentials expiring at {Expiry:o}", _expiresAtUtc);
    }

    private static DateTime ParseExpiration(JsonElement expiration)
    {
        if (expiration.ValueKind == JsonValueKind.String)
        {
            var value = expiration.GetString();

            if (DateTimeOffset.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out var timestamp))
            {
                return timestamp.UtcDateTime;
            }

            if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var unixTimestamp))
            {
                return FromUnixTimestamp(unixTimestamp);
            }
        }

        if (expiration.ValueKind == JsonValueKind.Number
            && expiration.TryGetDouble(out var numericTimestamp)
            && double.IsFinite(numericTimestamp)
            && numericTimestamp == Math.Truncate(numericTimestamp)
            && numericTimestamp >= long.MinValue
            && numericTimestamp <= long.MaxValue)
        {
            return FromUnixTimestamp((long)numericTimestamp);
        }

        throw new JsonException($"HMPPS token endpoint returned an invalid expiration value: {expiration}");
    }

    private static DateTime FromUnixTimestamp(long unixTimestamp) =>
        (unixTimestamp > 1_000_000_000_000
                ? DateTimeOffset.FromUnixTimeMilliseconds(unixTimestamp)
                : DateTimeOffset.FromUnixTimeSeconds(unixTimestamp))
            .UtcDateTime;

    /// <summary>
    /// Shape of the HMPPS token endpoint response. Adjust the JSON property names if the real contract
    /// differs.
    /// </summary>
    private sealed class AwsCredentialsResponse
    {
        [JsonPropertyName("accessKeyId")]
        public string AccessKeyId { get; init; } = string.Empty;

        [JsonPropertyName("secretAccessKey")]
        public string SecretAccessKey { get; init; } = string.Empty;

        [JsonPropertyName("sessionToken")]
        public string SessionToken { get; init; } = string.Empty;

        [JsonPropertyName("expiration")]
        public JsonElement Expiration { get; init; }
    }
}
