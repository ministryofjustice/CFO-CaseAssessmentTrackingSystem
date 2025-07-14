using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Providers;

public interface IPertinentEventProvider
{
    Task<IEnumerable<DipEventInformation>> GetEvents(string participantId, IApplicationDbContext context);
}