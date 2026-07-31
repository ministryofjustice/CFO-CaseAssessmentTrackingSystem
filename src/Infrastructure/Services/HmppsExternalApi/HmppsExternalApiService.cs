using System.Net;

namespace Cfo.Cats.Infrastructure.Services.HmppsExternalApi;

public class HmppsExternalApiService(HttpClient client, ILogger<HmppsExternalApiService> logger)
    : IHmppsExternalApiService
{
    public async Task<Result<string>> GetStatusAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Version segment is included per-endpoint rather than baked into BaseUrl.
            var response = await client.GetAsync("v1/status", cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return Result<string>.Success(content);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Result<string>.NotFound();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HMPPS External API is unavailable when calling status");
            return Result<string>.Failure("HMPPS External API is currently unavailable.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calling HMPPS External API status endpoint");
            return Result<string>.Failure("An unexpected error occurred while contacting the HMPPS External API.");
        }
    }
}
