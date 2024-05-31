using Cfo.Cats.Domain.Entities.Administration;
using DocumentFormat.OpenXml.Bibliography;

namespace Cfo.Cats.Application.Features.Locations.DTOs;

public record LocationDto
{
    public int Id { get; init; } 
    public required string Name { get; init; }
    public required string GenderProvision { get; init; } 
    
    public required string LocationType { get; init; }
    
    /// <summary>
    /// The parent location (currently only supported on community locations)
    /// </summary>
    public int? ParentLocationId { get; init; }
    
    public string? ParentLocationName { get; set; }

    /// <summary>
    /// Indicates whether the location is a custodial location or
    /// in the community (if false)
    /// </summary>
    public bool IsCustody { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Location, LocationDto>()
                .ForMember(t => t.Id, o => o.MapFrom(s => s.Id))
                .ForMember(t => t.GenderProvision, o => o.MapFrom(s => s.GenderProvision.Name))
                .ForMember(t => t.LocationType, o => o.MapFrom(s => s.LocationType.Name))
                .ForMember(t => t.ParentLocationId, o => o.MapFrom(s => s.ParentLocation!.Id))
                .ForMember(t => t.ParentLocationName, o => o.MapFrom(s => s.ParentLocation!.Name))
                .ForMember(t => t.Name, o => o.MapFrom(s => s.Name))
                .ForMember(t => t.IsCustody, o => o.MapFrom(_ => false));

                
        }
    }

}