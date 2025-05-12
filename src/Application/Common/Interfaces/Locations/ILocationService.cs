using Cfo.Cats.Application.Features.Locations.DTOs;

namespace Cfo.Cats.Application.Common.Interfaces.Locations;

public interface ILocationService
{
    IReadOnlyList<LocationDto> DataSource { get; }
    event Action? OnChange;
    void Refresh();
    IEnumerable<LocationDto> GetVisibleLocations(string tenantId);
}
