namespace Cfo.Cats.Application.Features.Participants.DTOs;

[Description("Participants")]
public class ParticipantPaginationDto
{
    [Description("Participant Id")]
    public string Id { get; set; } = default!;
    
    [Description("Status")]
    public EnrolmentStatus EnrolmentStatus { get; set; } = default!;
    
    [Description("Consent")]
    public ConsentStatus ConsentStatus { get; set; } = default!;
    
    [Description("Participant")]
    public string ParticipantName { get; set; } = default!;

    [Description("Location")]
    public string CurrentLocation { get; set; } = default!;

    [Description("Location Type")]
    public LocationType CurrentLocationType { get; set; } = default!;

    [Description("Enrolled At")]
    public string EnrolmentLocation { get; set; } = default!;

    [Description("Enrolment Location Type")]
    public LocationType EnrolmentLocationType { get; set; } = default!;

    [Description("Assignee")]
    public string Owner { get; set; } = default!;
    
    [Description("Tenant")]
    public string Tenant { get; set; } = default!;

    [Description("Risk Due")]
    public DateTime? RiskDue { get; set; }

    [Description] 
    public RiskDueReason RiskDueReason { get; set; } = RiskDueReason.Unknown;
}