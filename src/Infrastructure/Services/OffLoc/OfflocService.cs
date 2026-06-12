using System.Net;
using System.Net.Http.Json;
using Cfo.Cats.Application.Features.Offloc.DTOs;

namespace Cfo.Cats.Infrastructure.Services.OffLoc;

public class OfflocService(HttpClient client, ILogger<OfflocService> logger) : IOfflocService
{
    public async Task<Result<SentenceDataDto>> GetSentenceDataAsync(string nomsNumber)
    {
        try
        {
            var result = await client.GetFromJsonAsync<SentenceDataDto>($"/offloc/{nomsNumber}/sentences");
            
            if (result is null)
            {
                return Result<SentenceDataDto>.Failure("OffLoc returned no data.");
            }
            
            return result;
        }
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return Result<SentenceDataDto>.NotFound();
        }
        catch (HttpRequestException e)
        {
            logger.LogError(e, "Offloc service is unavailable when calling get sentence");
            return Result<SentenceDataDto>.Failure("OffLoc service is currently unavailable.");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error calling Offloc get sentence for NOMS: {NomsNumber}", nomsNumber);
            return Result<SentenceDataDto>.Failure("An unexpected error occurred while retrieving sentence data.");
        }
    }
}
