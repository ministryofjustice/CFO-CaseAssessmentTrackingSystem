namespace Cfo.Cats.Application.Features.Locations.DTOs;

public record LocationDto
{
    public int Id { get; set; } 
    public required string Name { get; set; }

    public required string ContractName { get; set; }
    public required GenderProvision GenderProvision { get; set; } 
    
    public required LocationType LocationType { get; set; }

    public string[] Tenants { get; set; } = [];
    
    /// <summary>
    /// The parent location (currently only supported on community locations)
    /// </summary>
    public int? ParentLocationId { get; init; }
    
    public string? ParentLocationName { get; set; }
   
}