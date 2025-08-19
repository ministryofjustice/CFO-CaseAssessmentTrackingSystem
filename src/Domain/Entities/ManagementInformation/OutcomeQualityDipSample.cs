using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Common.Exceptions;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class OutcomeQualityDipSample : BaseAuditableEntity<Guid>
{
#pragma warning disable CS8618 
    private OutcomeQualityDipSample() { /* this is for EF Core */}
#pragma warning restore CS8618

    private List<OutcomeQualityDipSampleParticipant> _participants = new();

    public static OutcomeQualityDipSample Create(string contractId, DateTime periodFrom, DateTime periodTo, int size = 0) => new()
    {
        Id = Guid.CreateVersion7(),
        ContractId = contractId,
        CreatedOn = DateTime.UtcNow,
        Status = DipSampleStatus.AwaitingReview,
        PeriodFrom = periodFrom,
        PeriodTo = periodTo,
        Size = size
    };

    public IReadOnlyCollection<OutcomeQualityDipSampleParticipant> Participants => _participants.AsReadOnly();

    /// <summary>
    /// Marks the dip sample as reviewd
    /// </summary>
    /// <param name="reviewedBy">The user who performed the review</param>
    /// <param name="noOfCompliant">The number of participants that are compliant</param>
    /// <returns>This item, with it's status changed</returns>
    /// <exception cref="MissingParticipantDetailsException">If the participants have not been loaded</exception>
    /// <exception cref="InvalidDipSampleTransitionException">If current status is not awaiting review, or any CSO answer is not answered</exception>
    public OutcomeQualityDipSample Review(string reviewedBy)
    {
        if(Participants.Count == 0)
        {
            throw new MissingParticipantDetailsException();
        }
                
        if (Status != DipSampleStatus.AwaitingReview)
        {
            throw new InvalidDipSampleTransitionException(Status, DipSampleStatus.Reviewed);
        }

        if (Participants.Any(p => p.CsoIsCompliant.IsAnswer == false))
        {
            throw new InvalidDipSampleTransitionException("All participants must have an answer to be reviewed");
        }

        ReviewedOn = DateTime.UtcNow;
        ReviewedBy = reviewedBy;
        Status = DipSampleStatus.Reviewed;
        SetCsoScores(Participants.Count(p => p.CsoIsCompliant.IsAccepted));

        return this;
    }

    /// <summary>
    /// Marks the record as verifying.
    /// </summary>
    /// <param name="reviewedBy">The user who performed the review</param>
    /// <returns>This item, with it's status changed</returns>
    /// <exception cref="MissingParticipantDetailsException">If the participants have not been loaded</exception>
    /// <exception cref="InvalidDipSampleTransitionException">If current status is not awaiting review, or any CPM answer is not answered</exception>
    public OutcomeQualityDipSample Verify(string userId)
    {
        if (Participants.Count == 0)
        {
            throw new MissingParticipantDetailsException();
        }

        if (Status != DipSampleStatus.Reviewed)
        {
            throw new InvalidDipSampleTransitionException(Status, DipSampleStatus.Verifying);
        }

        if (Participants.Any(p => p.CpmIsCompliant.IsAnswer == false && p.CsoIsCompliant == ComplianceAnswer.Unsure))
        {
            throw new InvalidDipSampleTransitionException("All participants must have an answer to be reviewed");
        }

        Status = DipSampleStatus.Verifying;
        AddDomainEvent(new OutcomeQualityDipSampleVerifyingDomainEvent(Id, userId, DateTime.UtcNow));
        return this;
    }

    /// <summary>
    /// Marks the sample as verified.
    /// </summary>
    /// <returns>This item, with it's status changed</returns>
    /// <exception cref="MissingParticipantDetailsException">If the participants have not been loaded</exception>
    /// <exception cref="InvalidDipSampleTransitionException">If current status is not awaiting review, or any CPM answer is not answered</exception>
    public OutcomeQualityDipSample Verified(string userId)
    {
        if (Participants.Count == 0)
        {
            throw new MissingParticipantDetailsException();
        }

        if (Status != DipSampleStatus.Verifying)
        {
            throw new InvalidDipSampleTransitionException(Status, DipSampleStatus.Verified);
        }

        if (Participants.Any(p => p.CpmIsCompliant.IsAnswer == false && p.CsoIsCompliant == ComplianceAnswer.Unsure))
        {
            throw new InvalidDipSampleTransitionException("All participants must have an answer to be reviewed");
        }

        foreach (var participant in Participants)
        {
            if (participant.CpmIsCompliant.IsAnswer == false)
            {
                if (participant.CpmIsCompliant.IsAnswer == false)
                {
                    var cpmAnswer = participant.CsoIsCompliant switch
                    {
                        var c when c.Name == ComplianceAnswer.Compliant.Name => ComplianceAnswer.AutoCompliant,
                        var c when c.Name == ComplianceAnswer.NotCompliant.Name => ComplianceAnswer.AutoNotCompliant,
                        _ => throw new ApplicationException("Invalid answer")
                    };

                    participant.CpmAnswer(
                        cpmAnswer,
                        participant.CsoComments!,
                        userId,
                        DateTime.UtcNow
                    );
                }

            }

            if (participant.CsoIsCompliant.IsAccepted == participant.CpmIsCompliant.IsAccepted)
            {
                participant.FinalAnswer(participant.CpmIsCompliant, participant.CpmComments!, participant.CpmReviewedBy!,
                    DateTime.UtcNow);
            }
        }

        Status = DipSampleStatus.Verified;
        CpmCompliant = Participants.Count(p => p.CpmIsCompliant.IsAccepted);
        CpmPercentage = CalculatePercentage(CpmCompliant.Value);
        CpmScore = CalculateScore(CpmPercentage.Value);
        return this;
    }

    /// <summary>
    /// Marks the sample as finalising
    /// </summary>
    /// <param name="userId">The user performing the verification</param>
    /// <returns>This item with it's status change</returns>
    /// <exception cref="InvalidDipSampleTransitionException">If the status change is not valid</exception>
    public OutcomeQualityDipSample Finalise(string userId)
    {
        if (Status != DipSampleStatus.Verified)
        {
            throw new InvalidDipSampleTransitionException(Status, DipSampleStatus.Finalising);
        }

        Status = DipSampleStatus.Finalising;
        AddDomainEvent(new OutcomeQualityDipSampleFinalisingDomainEvent(Id, userId, DateTime.UtcNow));
        return this;
    }

    /// <summary>
    /// Reverts the sample from finalising to Verified in the event of a failure
    /// </summary>
    /// <param name="userId">The user performing the verification</param>
    /// <returns>This item with it's status change</returns>
    /// <exception cref="InvalidDipSampleTransitionException">If the status change is not valid</exception>
    public OutcomeQualityDipSample FinalisationFailed()
    {
        if (Status != DipSampleStatus.Finalising)
        {
            throw new InvalidDipSampleTransitionException(Status, DipSampleStatus.Verified);
        }
        Status = DipSampleStatus.Verified;
        return this;
    }

    /// <summary>
    /// Marks the sample as finalised.
    /// </summary>
    /// <param name="noOfCompliant">The no of participants who's state is "finalised"</param>
    /// <returns>This item with it's status change</returns>
    /// <exception cref="InvalidDipSampleTransitionException">If the status change is not valid</exception>
    public OutcomeQualityDipSample Finalised(int noOfCompliant)
    {
        if (Status != DipSampleStatus.Finalising)
        {
            throw new InvalidDipSampleTransitionException(Status, DipSampleStatus.Finalised);
        }
        Status = DipSampleStatus.Finalised;
        SetFinalScores(noOfCompliant);
        return this;
    }

    private OutcomeQualityDipSample SetFinalScores(int noOfCompliant)
    {
        FinalCompliant = noOfCompliant;
        FinalPercentage = CalculatePercentage(noOfCompliant);
        FinalScore = CalculateScore(FinalPercentage.Value);

        return this;
    }

    public string ContractId { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime PeriodFrom { get; private set; }
    public DateTime PeriodTo { get; private set; }
    public DateTime? ReviewedOn { get; private set; }
    public string? ReviewedBy { get; private set; }
    public DipSampleStatus Status { get; private set; }

    /// <summary>
    /// The size of the sample.
    /// </summary>
    public int Size { get; private set; }

    /// <summary>
    /// Count of <see cref="ComplianceAnswer.Compliant"/> records marked by the CSO.
    /// </summary>
    public int? CsoCompliant { get; private set; }

    /// <summary>
    /// Calculated as (<see cref="CsoCompliant"/> ÷ <see cref="Size"/>).
    /// </summary>
    public int? CsoPercentage { get; private set; }

    /// <summary>
    /// Derived from <see cref="CsoPercentage"/>, ranges from <c>0</c> (lowest) to <c>5</c> (highest).
    /// </summary>
    public int? CsoScore { get; private set; }

    /// <summary>
    /// Count of <see cref="ComplianceAnswer.Compliant"/> or <see cref="ComplianceAnswer.AutoCompliant"/> records marked by the CSO/CPM.
    /// </summary>
    public int? CpmCompliant { get; private set; }

    /// <summary>
    /// Calculated as (<see cref="CpmCompliant"/> ÷ <see cref="Size"/>).
    /// </summary>
    public int? CpmPercentage { get; private set; }

    /// <summary>
    /// Derived from <see cref="CpmPercentage"/>, ranges from <c>0</c> (lowest) to <c>5</c> (highest).
    /// </summary>
    public int? CpmScore { get; private set; }

    private void SetCsoScores(int noOfCompliant)
    {
        CsoCompliant = noOfCompliant;
        CsoPercentage = CalculatePercentage(noOfCompliant);
        CsoScore = CalculateScore(CsoPercentage.Value);
    }
    
    

    #region Calculated business (final) properties
    /// <summary>
    /// Count of <see cref="ComplianceAnswer.Compliant"/> or <see cref="ComplianceAnswer.AutoCompliant"/> records marked by the business.
    /// </summary>
    public int? FinalCompliant { get; private set; }

    /// <summary>
    /// Calculated as (<see cref="FinalCompliant"/> ÷ <see cref="Size"/>).
    /// </summary>
    public int? FinalPercentage { get; private set; }

    /// <summary>
    /// Derived from <see cref="FinalPercentage"/>, ranges from <c>0</c> (lowest) to <c>5</c> (highest).
    /// </summary>
    public int? FinalScore { get; private set; }
    #endregion

    private int CalculatePercentage(int noOfCompliant) => (int)Math.Ceiling((double)noOfCompliant / Size * 100);

    private static int CalculateScore(int percentage) => percentage switch
    {
        >= 91 => 5,
        >= 81 => 4,
        >= 71 => 3,
        >= 61 => 2,
        >= 51 => 1,
        _ => 0
    };
}
