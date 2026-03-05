using Cfo.Cats.Domain.Labels;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Labels.DTOs;

public record LabelDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    
    [JsonIgnore] // cannot serialize a smart enum easily
    public required LabelScope Scope { get; init; }
    
    public AppColour Colour { get; init; }
    
    public AppVariant Variant { get; init; } 

    public AppIcon AppIcon { get; init; }
    
    public required string Contract { get; init; }
}