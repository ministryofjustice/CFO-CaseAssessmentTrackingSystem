using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Locations.DTOs;

public static class LocationMappings
{
    public static readonly Expression<Func<Location, LocationDto>> ToDto =
        static entity => new()
        {
            Id = entity.Id,
            GenderProvision = entity.GenderProvision,
            LocationType = entity.LocationType,
            ParentLocationId = entity.ParentLocation != null ? entity.ParentLocation.Id : null,
            ParentLocationName = entity.ParentLocation != null ? entity.ParentLocation.Name : null,
            Name = entity.Name,
            Tenants = entity.Tenants.Select(t => t.Id).ToArray(),
            ContractName = entity.Contract != null ? entity.Contract.Description : string.Empty
        };
}

