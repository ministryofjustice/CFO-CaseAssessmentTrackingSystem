using System.Net;

namespace Cfo.Cats.Infrastructure.Services.ExternalEvents;

/// <summary>
/// Retrieves authoritative HMPPS event data from a notification URL using an <see cref="HttpClient"/>
/// pre-configured with the HMPPS mutual TLS certificate and API key.
/// </summary>
public class HmppsEventDataRetriever(HttpClient client, ILogger<HmppsEventDataRetriever> logger)
    : IHmppsEventDataRetriever
{
    public async Task<string?> RetrieveAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            var response = await client.GetAsync(url, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to retrieve HMPPS event data from {Url}", url);
            throw;
        }
    }
}
