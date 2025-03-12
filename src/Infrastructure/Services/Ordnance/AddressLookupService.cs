using Cfo.Cats.Application.Features.Participants.DTOs;
using System.Net.Http.Json;

namespace Cfo.Cats.Infrastructure.Services.Ordnance;

public class AddressLookupService(
    HttpClient client) : IAddressLookupService
{
    public async Task<Result<IEnumerable<ParticipantAddressDto>>> SearchAsync(string query, CancellationToken cancellationToken)
    {
        var queryParams = new[]
        {
            "lr=EN",
            $"query={query}",
            "maxresults=15"
        };

        IEnumerable<OrdnanceResult> results = [];

        try
        {
            var result = await client.GetFromJsonAsync<OrdnanceResponse>($"find?{string.Join('&', queryParams)}", cancellationToken);

            if (result is { Error: not null })
            {
                return Result<IEnumerable<ParticipantAddressDto>>.Failure($"Could not obtain address information ({result.Error.StatusCode})");
            }

            results = result!.Results ?? [];
        }
        catch
        {
            return Result<IEnumerable<ParticipantAddressDto>>.Failure("A fatal error occurred while obtaining address information");
        }

        return Result<IEnumerable<ParticipantAddressDto>>.Success(results.Select(x => x.DPA));
    }
}

public interface IAddressLookupService
{
    Task<Result<IEnumerable<ParticipantAddressDto>>> SearchAsync(string query, CancellationToken cancellationToken);
}

public record class OrdnanceResponse
{
    public IEnumerable<OrdnanceResult>? Results { get; set; }
    public OrdnanceError? Error { get; set; }

}

public record class OrdnanceResult
{
    public required ParticipantAddressDto DPA { get; set; }
}

public record class OrdnanceError
{
    public required int StatusCode { get; set; }
    public required string Message { get; set; }
}