using System.Net;
using Cfo.Cats.Application.Features.Participants.DTOs;
using System.Net.Http.Json;

namespace Cfo.Cats.Infrastructure.Services.Ordnance;

public class AddressLookupService(
    HttpClient client,
    ILogger<AddressLookupService> logger) : IAddressLookupService
{
    public async Task<Result<IEnumerable<ParticipantAddressDto>>> SearchAsync(string query,
        CancellationToken cancellationToken)
    {
        var queryParams = new[]
        {
            "lr=EN",
            $"query={Uri.EscapeDataString(query)}", // URL-safe parsing
            "maxresults=15"
        };

        try
        {
            var result =
                await client.GetFromJsonAsync<OrdnanceResponse>($"find?{string.Join('&', queryParams)}",
                    cancellationToken);

            if (result is null)
            {
                return Result<IEnumerable<ParticipantAddressDto>>.Failure("Address lookup returned no data.");
            }

            if (result.Error is not null)
            {
                logger.LogWarning("Ordnance API returned an error: {StatusCode} - {Message}", result.Error.StatusCode,
                    result.Error.Message);
                return Result<IEnumerable<ParticipantAddressDto>>.Failure(
                    $"Could not obtain address information ({result.Error.StatusCode})");
            }

            var results = result.Results ?? Array.Empty<OrdnanceResult>();

            // Map to DTO array/list immediately to evaluate the selection safely inside the try block
            var addresses = results.Select(x => x.DPA).ToArray();

            return Result<IEnumerable<ParticipantAddressDto>>.Success(addresses);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Result<IEnumerable<ParticipantAddressDto>>.NotFound();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Ordnance API service is unavailable or returned network error.");
            return Result<IEnumerable<ParticipantAddressDto>>.Failure(
                "Address lookup service is currently unavailable.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while obtaining address information for query: {Query}", query);
            return Result<IEnumerable<ParticipantAddressDto>>.Failure(
                "A fatal error occurred while obtaining address information.");
        }
    }
}

public interface IAddressLookupService
{
    Task<Result<IEnumerable<ParticipantAddressDto>>> SearchAsync(string query, CancellationToken cancellationToken);
}

public record OrdnanceResponse
{
    public IEnumerable<OrdnanceResult>? Results { get; init; }
    public OrdnanceError? Error { get; init; }
}

public record OrdnanceResult
{
    public required ParticipantAddressDto DPA { get; set; }
}

public record OrdnanceError
{
    public required int StatusCode { get; set; }
    public required string Message { get; set; }
}
