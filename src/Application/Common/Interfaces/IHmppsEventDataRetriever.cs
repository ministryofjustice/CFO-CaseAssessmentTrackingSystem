namespace Cfo.Cats.Application.Common.Interfaces;

/// <summary>
/// Retrieves the authoritative data for an HMPPS external event from the URL supplied in the
/// notification. The implementation hides the HMPPS-specific transport concerns (mutual TLS client
/// certificate, API key) from the rest of the application.
/// </summary>
public interface IHmppsEventDataRetriever
{
    /// <summary>
    /// Performs an authenticated GET against <paramref name="url"/> and returns the raw response body,
    /// or <c>null</c> if the resource could not be found.
    /// </summary>
    Task<string?> RetrieveAsync(string url, CancellationToken cancellationToken);
}
