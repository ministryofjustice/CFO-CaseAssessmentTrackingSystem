namespace Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

public class DipEventInformation
{
    public required string Title { get; init; }
    
    public required string[] Contents { get; init; }
    public required string ActionedBy { get; init; }
    public required DateTime OccurredOn { get; init; }
    public required DateTime RecordedOn { get; init; }
    public AppColour Colour { get; set; } = AppColour.Info;
    
    public required AppIcon Icon { get; init; }
}
