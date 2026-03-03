using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.ParticipantLabels.GetParticipantLabels;

public record ParticipantLabelDto
{
    public Guid ParticipantLabelId { get; init;}
    public Guid LabelId { get; init;}
    public required string Name { get; init; }
    public required string Description { get; init; }
    
    public required LabelScope Scope { get; init; }
    public AppColour Colour { get; init; }
    public AppVariant Variant { get; init; } 
    public AppIcon AppIcon { get; init; }
    public required DateTime LifetimeStart { get; init;}
    public required DateTime LifetimeEnd {get; init;}
    public required string AddedBy { get; init;}
    public required string ClosedBy { get; init;}
    
}