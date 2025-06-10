using Cfo.Cats.Application.Features.Delius.DTOs;

namespace Cfo.Cats.Application.Common.Interfaces;

public interface IDeliusService
{
    Task<Result<OffenceDto>> GetOffencesAsync(string crn);
    Task<Result<OffenderManagerSummaryDto>> GetOffenderManagerSummaryAsync(string crn);
}