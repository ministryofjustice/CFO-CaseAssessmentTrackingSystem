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
            return result!;
        }
        catch(HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Result<OffenceDto>.NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calling get offences");
            return Result<OffenceDto>.Failure("Failed to retrieve information");
        }
    }
    public async Task<Result<OffenderManagerSummaryDto>> GetOffenderManagerSummaryAsync(string crn)
    {
        try
        {
            var result = await client.GetFromJsonAsync<OffenderManagerSummaryDto>($"/delius/{crn}/offendermanagersummary");
            return result!;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Result<OffenderManagerSummaryDto>.NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calling get offender manager Org/team details");
            return Result<OffenderManagerSummaryDto>.Failure("Failed to retrieve information");
        }
    }
}