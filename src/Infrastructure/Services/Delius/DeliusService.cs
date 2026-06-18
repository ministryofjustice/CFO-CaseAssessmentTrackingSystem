using System.Net;
using System.Net.Http.Json;
using Cfo.Cats.Application.Features.Delius.DTOs;

namespace Cfo.Cats.Infrastructure.Services.Delius;

public class DeliusService(HttpClient client, ILogger<DeliusService> logger) : IDeliusService
{
    public async Task<Result<OffenceDto>> GetOffencesAsync(string crn)
    {
        try
        {
            var result = await client.GetFromJsonAsync<OffenceDto>($"/delius/{crn}/offences");
            
            if (result is null)
            {
                return Result<OffenceDto>.Failure("Delius returned no data.");
            }

            return result;
        }
        catch(HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Result<OffenceDto>.NotFound();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Delius service is unavailable when calling get offences");
            return Result<OffenceDto>.Failure("Delius service is currently unavailable.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calling Delius get offences for CRN: {Crn}", crn);
            return Result<OffenceDto>.Failure("An unexpected error occurred while retrieving offence data.");
        }
    }
    
    public async Task<Result<OffenderManagerSummaryDto>> GetOffenderManagerSummaryAsync(string crn)
    {
        try
        {
            var result = await client.GetFromJsonAsync<OffenderManagerSummaryDto>($"/delius/{crn}/offendermanagersummary");
            
            if (result is null)
            {
                return Result<OffenderManagerSummaryDto>.Failure("Delius returned no data.");
            }
            
            return result;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Result<OffenderManagerSummaryDto>.NotFound();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Delius service is unavailable when calling get offender manager summary");
            return Result<OffenderManagerSummaryDto>.Failure("Delius service is currently unavailable.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calling Delius get offender manager summary");
            return Result<OffenderManagerSummaryDto>.Failure("An unexpected error occurred while retrieving offender manager information.");
        }
    }
}
