using System.Net;
using System.Net.Http.Json;
using Cfo.Cats.Application.Features.Offloc.DTOs;

namespace Cfo.Cats.Infrastructure.Services.OffLoc;

public class OfflocService(HttpClient client) : IOfflocService
{
    public async Task<Result<PersonalDetailsDto>> GetPersonalDetailsAsync(string nomisNumber)
    {
        try
        {
            var result = await client.GetFromJsonAsync<PersonalDetailsDto>($"/offloc/{nomisNumber}");
            return result!;
        }
        catch(HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return Result<PersonalDetailsDto>.NotFound();
        }
        catch (Exception e)
        {
            return Result<PersonalDetailsDto>.Failure(e.Message);
        }

    }
}