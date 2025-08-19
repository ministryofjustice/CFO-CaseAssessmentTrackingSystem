namespace Cfo.Cats.Application.Features.Activities.DTOs;

public class ActivitySummaryDto
{
    public required Guid Id { get; set; }
    public required string TookPlaceAtLocationName { get; set; }
    public required Guid TaskId { get; set; }
    public required DateTime Created { get; set; }
    public required DateTime CommencedOn { get; set; }
    public required ActivityDefinition Definition { get; set; }
    public required ActivityStatus Status { get; set; }
    public required DateTime Expiry { get; set; }

    [Description("Additional Information")]
    public string? AdditionalInformation { get; set; }
}
