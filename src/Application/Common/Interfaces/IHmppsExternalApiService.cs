namespace Cfo.Cats.Application.Common.Interfaces;

/// <summary>
/// Client for the HMPPS External API, authenticated via mutual TLS and an API key.
/// </summary>
public interface IHmppsExternalApiService
{
    /// <summary>
    /// Calls the API's versioned status endpoint. Useful for verifying that the mTLS handshake and
    /// API key are configured correctly end-to-end.
    /// </summary>
    Task<Result<string>> GetStatusAsync(CancellationToken cancellationToken);
}
