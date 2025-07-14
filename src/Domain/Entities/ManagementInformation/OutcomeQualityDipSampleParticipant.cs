namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class OutcomeQualityDipSampleParticipant
{
#pragma warning disable CS8618 
    public OutcomeQualityDipSampleParticipant() { /* this is for EF Core */}
#pragma warning restore CS8618

    public string ParticipantId { get; set; }
    public Guid DipSampleId { get; set; }

    public string LocationType { get; set; }
    public bool? HasClearParticipantJourney { get; set; }
    public bool? ShowsTaskProgression { get; set; }
    public bool? ActivitiesLinkToTasks { get; set; }
    public bool? TTGDemonstratesGoodPRIProcess { get; set; }
    public bool? TemplatesAlignWithREG { get; set; }
    public bool? SupportsJourneyAndAlignsWithDoS { get; set; }
    public string? Remarks { get; set; }
    public bool? IsCompliant { get; set; }
    public DateTime? ReviewedOn { get; set; }
    public string? ReviewedBy { get; set; }
}
