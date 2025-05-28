using System.Net;
using System.Net.Http.Json;
using Cfo.Cats.Application.Features.Offloc.DTOs;

namespace Cfo.Cats.Infrastructure.Services.OffLoc;

public class OfflocService(HttpClient client) : IOfflocService
{
    public async Task<Result<SentenceDataDto>> GetSentenceDataAsync(string nomsNumber)
    {
        try
        {
            var result = await client.GetFromJsonAsync<SentenceDataDto>($"/offloc/{nomsNumber}/sentences");
            return result!;
        }
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return Result<SentenceDataDto>.NotFound();
        }
        catch (Exception e)
        {
            return Result<SentenceDataDto>.Failure(e.Message);
        }
    }
}