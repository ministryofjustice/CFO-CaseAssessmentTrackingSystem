using Cfo.Cats.Domain.Entities.Administration;
using DocumentFormat.OpenXml.Bibliography;

namespace Cfo.Cats.Application.Features.Locations.DTOs;

public record LocationDto
{
    public int Id { get; init; } 
    public required string Name { get; init; }
    public required GenderProvision GenderProvision { get; init; } 
    
    public required LocationType LocationType { get; init; }
    
    /// <summary>
    /// The parent location (currently only supported on community locations)
    /// </summary>
    public int? ParentLocationId { get; init; }
    
    public string? ParentLocationName { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Location, LocationDto>()
                .ForMember(t => t.Id, o => o.MapFrom(s => s.Id))
                .ForMember(t => t.GenderProvision, o => o.MapFrom(s => s.GenderProvision))
                .ForMember(t => t.LocationType, o => o.MapFrom(s => s.LocationType))
                .ForMember(t => t.ParentLocationId, o => o.MapFrom(s => s.ParentLocation!.Id))
                .ForMember(t => t.ParentLocationName, o => o.MapFrom(s => s.ParentLocation!.Name))
                .ForMember(t => t.Name, o => o.MapFrom(s => s.Name));
        }
    }

 

}