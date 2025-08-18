using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class OutcomeQualityDipSampleParticipant : BaseAuditableEntity<int>
{

#pragma warning disable CS8618 
    private OutcomeQualityDipSampleParticipant() { /* this is for EF Core */}
#pragma warning restore CS8618

    public static OutcomeQualityDipSampleParticipant Create(Guid sampleId, string participantId, string locationType)
    {
        return new OutcomeQualityDipSampleParticipant()
        {
            DipSampleId = sampleId,
            ParticipantId = participantId,
            LocationType = locationType
        };
    }

    public string ParticipantId { get; private set; }
    public Guid DipSampleId { get; private set; }

    public string LocationType { get; private set; }
    public DipSampleAnswer HasClearParticipantJourney { get; private set; } = DipSampleAnswer.NotAnswered;
    public DipSampleAnswer ShowsTaskProgression { get; private set; } = DipSampleAnswer.NotAnswered;
    public DipSampleAnswer ActivitiesLinkToTasks { get; private set; } = DipSampleAnswer.NotAnswered;
    public DipSampleAnswer TTGDemonstratesGoodPRIProcess { get; private set; } = DipSampleAnswer.NotAnswered;
    public DipSampleAnswer SupportsJourney { get; private set; } = DipSampleAnswer.NotAnswered;
    public DipSampleAnswer AlignsWithDoS { get; private set; } = DipSampleAnswer.NotAnswered;

    public ComplianceAnswer CsoIsCompliant { get; private set; } = ComplianceAnswer.NotAnswered;
    public DateTime? CsoReviewedOn { get; private set; }
    public string? CsoReviewedBy { get; private set; }
    public string? CsoComments { get; private set; }
    

    public ComplianceAnswer CpmIsCompliant { get; private set; } = ComplianceAnswer.NotAnswered;
    public DateTime? CpmReviewedOn { get; private set; }
    public string? CpmReviewedBy { get; private set; }
    public string? CpmComments { get; private set; }

    public ComplianceAnswer FinalIsCompliant { get; private set; } = ComplianceAnswer.NotAnswered;
    public DateTime? FinalReviewedOn { get; private set; }
    public string? FinalReviewedBy { get; private set; }
    public string? FinalComments { get; private set; }

    public OutcomeQualityDipSampleParticipant CsoAnswer(
        DipSampleAnswer clearJourney,
        DipSampleAnswer taskProgression,
        DipSampleAnswer linksToTasks,
        DipSampleAnswer ttgDemonstratesGoodPRIProcess,
        DipSampleAnswer supportsJourney,
        DipSampleAnswer alignsWithDoS,
        ComplianceAnswer isCompliant,
        string comments,
        string reviewBy,
        DateTime reviewedOn
        )
    {

        if (FinalIsCompliant.IsAnswer)
        {
            throw new ApplicationException("Cannot answer a closed sample");
        }
        
        
        HasClearParticipantJourney = clearJourney;
        ShowsTaskProgression = taskProgression;
        ActivitiesLinkToTasks = linksToTasks;
        TTGDemonstratesGoodPRIProcess = ttgDemonstratesGoodPRIProcess;
        SupportsJourney = supportsJourney;
        AlignsWithDoS = alignsWithDoS;
        CsoIsCompliant = isCompliant;
        CsoComments = comments;
        CsoReviewedBy = reviewBy;
        CsoReviewedOn = reviewedOn;

        AddDomainEvent(new OutcomeQualityDipSampleParticipantScoredDomainEvent(DipSampleId, reviewBy, isCompliant.IsAccepted));
        
        return this;
    }

    public OutcomeQualityDipSampleParticipant CpmAnswer(
        ComplianceAnswer isCompliant,
        string comments,
        string reviewBy,
        DateTime reviewedOn)
    {
        CpmIsCompliant = isCompliant;
        CpmComments = comments;
        CpmReviewedBy = reviewBy;
        CpmReviewedOn = reviewedOn;

        return this;
    }

    public OutcomeQualityDipSampleParticipant FinalAnswer(
        ComplianceAnswer isCompliant,
        string comments,
        string reviewedBy,
        DateTime reviewedOn
    )
    {
        //TODO: Add validation around here
        FinalIsCompliant = isCompliant;
        FinalComments = comments;
        FinalReviewedBy = reviewedBy;
        FinalReviewedOn = reviewedOn;

        return this;
    }

}
