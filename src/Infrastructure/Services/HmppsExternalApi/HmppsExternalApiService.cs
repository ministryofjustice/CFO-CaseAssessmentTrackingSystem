using System.Net;
using System.Net.Http.Json;
using Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;

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

    public async Task<Result<RisksDto>> GetSeriousHarmRiskAsync(string hmppsId, CancellationToken cancellationToken)
    {
        try
        {
            // Version segment is included per-endpoint rather than baked into BaseUrl.
            var response = await client.GetFromJsonAsync<DataResponse<RisksDto>>(
                $"v1/persons/{Uri.EscapeDataString(hmppsId)}/risks/serious-harm", cancellationToken);

            if (response?.Data is null)
            {
                return Result<RisksDto>.Failure("HMPPS External API returned no data.");
            }

            return Result<RisksDto>.Success(response.Data);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Result<RisksDto>.NotFound();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HMPPS External API is unavailable when calling serious harm risk for HMPPS ID: {HmppsId}", hmppsId);
            return Result<RisksDto>.Failure("HMPPS External API is currently unavailable.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calling HMPPS External API serious harm risk endpoint for HMPPS ID: {HmppsId}", hmppsId);
            return Result<RisksDto>.Failure("An unexpected error occurred while contacting the HMPPS External API.");
        }
    }

    /// <summary>
    /// Envelope used by the HMPPS External API to wrap response payloads, e.g. <c>{ "data": { ... } }</c>.
    /// </summary>
    private class DataResponse<T>
    {
        public T? Data { get; init; }
    }
}
