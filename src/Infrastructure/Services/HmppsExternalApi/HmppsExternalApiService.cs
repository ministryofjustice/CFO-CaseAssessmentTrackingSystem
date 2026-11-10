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

    public async Task<Result<PersonDto>> GetPersonAsync(string hmppsId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await client.GetFromJsonAsync<DataResponse<PersonDto>>(
                $"v2/persons/{Uri.EscapeDataString(hmppsId)}", cancellationToken);

            if (response?.Data is null)
            {
                return Result<PersonDto>.Failure("HMPPS External API returned no data.");
            }

            return Result<PersonDto>.Success(response.Data);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Result<PersonDto>.NotFound();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HMPPS External API is unavailable when calling person for HMPPS ID: {HmppsId}", hmppsId);
            return Result<PersonDto>.Failure("HMPPS External API is currently unavailable.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calling HMPPS External API person endpoint for HMPPS ID: {HmppsId}", hmppsId);
            return Result<PersonDto>.Failure("An unexpected error occurred while contacting the HMPPS External API.");
        }
    }

    public async Task<Result<TierDetailsDto>> GetTierDetailsAsync(string hmppsId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await client.GetFromJsonAsync<DataResponse<TierDetailsDto>>(
                $"v1/persons/{Uri.EscapeDataString(hmppsId)}/tier/details", cancellationToken);

            if (response?.Data is null)
            {
                return Result<TierDetailsDto>.Failure("HMPPS External API returned no data.");
            }

            return Result<TierDetailsDto>.Success(response.Data);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Result<TierDetailsDto>.NotFound();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HMPPS External API is unavailable when calling tier details for HMPPS ID: {HmppsId}", hmppsId);
            return Result<TierDetailsDto>.Failure("HMPPS External API is currently unavailable.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calling HMPPS External API tier details endpoint for HMPPS ID: {HmppsId}", hmppsId);
            return Result<TierDetailsDto>.Failure("An unexpected error occurred while contacting the HMPPS External API.");
        }
    }

    public Task<Result<OffenceDto[]>> GetOffencesAsync(string hmppsId, CancellationToken cancellationToken)
        => GetAllPagesAsync<OffenceDto>(hmppsId, "offences", cancellationToken);

    public Task<Result<SentenceDto[]>> GetSentencesAsync(string hmppsId, CancellationToken cancellationToken)
        => GetAllPagesAsync<SentenceDto>(hmppsId, "sentences", cancellationToken);

    public Task<Result<SentenceKeyDatesDto>> GetLatestSentenceKeyDatesAsync(string hmppsId, CancellationToken cancellationToken)
        => GetDataAsync<SentenceKeyDatesDto>(hmppsId, "sentences/latest-key-dates-and-adjustments", cancellationToken);

    public Task<Result<PersonLicencesDto>> GetLicenceConditionsAsync(string hmppsId, CancellationToken cancellationToken)
        => GetDataAsync<PersonLicencesDto>(hmppsId, "licences/conditions", cancellationToken);

    public Task<Result<PersonResponsibleOfficerDto>> GetPersonResponsibleOfficerAsync(string hmppsId, CancellationToken cancellationToken)
        => GetDataAsync<PersonResponsibleOfficerDto>(hmppsId, "person-responsible-officer", cancellationToken);

    private async Task<Result<T>> GetDataAsync<T>(string hmppsId, string resource, CancellationToken cancellationToken)
        where T : class
    {
        try
        {
            var response = await client.GetFromJsonAsync<DataResponse<T>>(
                $"v1/persons/{Uri.EscapeDataString(hmppsId)}/{resource}", cancellationToken);

            if (response?.Data is null)
            {
                return Result<T>.Failure("HMPPS External API returned no data.");
            }

            return Result<T>.Success(response.Data);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Result<T>.NotFound();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HMPPS External API is unavailable when calling {Resource} for HMPPS ID: {HmppsId}", resource, hmppsId);
            return Result<T>.Failure("HMPPS External API is currently unavailable.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calling HMPPS External API {Resource} endpoint for HMPPS ID: {HmppsId}", resource, hmppsId);
            return Result<T>.Failure("An unexpected error occurred while contacting the HMPPS External API.");
        }
    }

    private const int PageSize = 100;

    // Guards against an upstream pagination bug causing an unbounded loop.
    private const int MaxPages = 50;

    private async Task<Result<T[]>> GetAllPagesAsync<T>(string hmppsId, string resource, CancellationToken cancellationToken)
    {
        try
        {
            var items = new List<T>();

            for (var page = 1; page <= MaxPages; page++)
            {
                var response = await client.GetFromJsonAsync<PaginatedResponse<T>>(
                    $"v1/persons/{Uri.EscapeDataString(hmppsId)}/{resource}?page={page}&perPage={PageSize}", cancellationToken);

                if (response?.Data is null)
                {
                    return Result<T[]>.Failure("HMPPS External API returned no data.");
                }

                items.AddRange(response.Data);

                if (response.Pagination?.IsLastPage != false || response.Data.Count == 0)
                {
                    break;
                }
            }

            return Result<T[]>.Success(items.ToArray());
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Result<T[]>.NotFound();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HMPPS External API is unavailable when calling {Resource} for HMPPS ID: {HmppsId}", resource, hmppsId);
            return Result<T[]>.Failure("HMPPS External API is currently unavailable.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calling HMPPS External API {Resource} endpoint for HMPPS ID: {HmppsId}", resource, hmppsId);
            return Result<T[]>.Failure("An unexpected error occurred while contacting the HMPPS External API.");
        }
    }

    /// <summary>
    /// Envelope used by the HMPPS External API to wrap response payloads, e.g. <c>{ "data": { ... } }</c>.
    /// </summary>
    private class DataResponse<T>
    {
        public T? Data { get; init; }
    }

    /// <summary>
    /// Envelope used by the HMPPS External API for paged list endpoints, e.g. <c>{ "data": [ ... ], "pagination": { ... } }</c>.
    /// </summary>
    private class PaginatedResponse<T>
    {
        public List<T>? Data { get; init; }
        public PaginationDto? Pagination { get; init; }
    }

    private class PaginationDto
    {
        public bool? IsLastPage { get; init; }
    }
}
