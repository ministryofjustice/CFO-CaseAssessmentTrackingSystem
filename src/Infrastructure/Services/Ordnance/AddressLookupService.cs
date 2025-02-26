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

        var result = await client.GetFromJsonAsync<OrdnanceResponse>($"find?{string.Join('&', queryParams)}", cancellationToken);

        if(result is { Error: not null })
        {
            // Error occurred
            throw new Exception(result.Error.Message);
        }

        IEnumerable<OrdnanceResult> results = result!.Results ?? [];

        return Result<IEnumerable<ParticipantAddressDto>>.Success(results.Select(x => x.DPA));
    }

    public async Task<Result<IEnumerable<ParticipantAddressDto>>> SearchByPostCodeAsync(string postCode, CancellationToken cancellationToken)
    {
        var queryParams = new[]
        {
            "lr=EN",
            $"postcode={postCode}"
        };

        var result = await client.GetFromJsonAsync<dynamic>($"/postcode?{string.Join('&', queryParams)}", cancellationToken);

        return Result<IEnumerable<ParticipantAddressDto>>.Success([]);
    }
}

public interface IAddressLookupService
{
    Task<Result<IEnumerable<ParticipantAddressDto>>> SearchAsync(string query, CancellationToken cancellationToken);
    Task<Result<IEnumerable<ParticipantAddressDto>>> SearchByPostCodeAsync(string postCode, CancellationToken cancellationToken);
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