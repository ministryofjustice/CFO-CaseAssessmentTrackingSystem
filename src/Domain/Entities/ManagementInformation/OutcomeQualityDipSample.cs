using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class OutcomeQualityDipSample : BaseAuditableEntity<Guid>
{
#pragma warning disable CS8618 
    private OutcomeQualityDipSample() { /* this is for EF Core */}
#pragma warning restore CS8618

    public static OutcomeQualityDipSample Create(string contractId, DateTime periodFrom, DateTime periodTo, int size = 0) => new()
    {
        Id = Guid.CreateVersion7(),
        ContractId = contractId,
        CreatedOn = DateTime.UtcNow,
        Status = DipSampleStatus.InProgress,
        PeriodFrom = periodFrom,
        PeriodTo = periodTo,
        Size = size
    };

    public OutcomeQualityDipSample Complete(string completedBy, int noOfCompliant = 0)
    {
        CompletedOn = DateTime.UtcNow;
        CompletedBy = completedBy;
        Status = DipSampleStatus.Completed;
        SetCsoScores(noOfCompliant);

        return this;
    }

    public string ContractId { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime PeriodFrom { get; private set; }
    public DateTime PeriodTo { get; private set; }
    public DateTime? CompletedOn { get; private set; }
    public string? CompletedBy { get; private set; }
    public DipSampleStatus Status { get; private set; }

    /// <summary>
    /// The size of the sample.
    /// </summary>
    public int Size { get; private set; }

    #region Calculated CSO properties
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

    private void SetCsoScores(int noOfCompliant)
    {
        CsoCompliant = noOfCompliant;
        CsoPercentage = CalculatePercentage(noOfCompliant);
        CsoScore = CalculateScore(CsoPercentage.Value);
    }

    #endregion

    #region Calculated CPM properties
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

    public OutcomeQualityDipSample SetCpmScores(int noOfCompliant)
    {
        CpmCompliant = noOfCompliant;
        CpmPercentage = CalculatePercentage(noOfCompliant);
        CpmScore = CalculateScore(CpmPercentage.Value);

        return this;
    }
    #endregion

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

    int CalculatePercentage(int noOfCompliant) => (int)Math.Ceiling((double)noOfCompliant / Size * 100);

    static int CalculateScore(int percentage) => percentage switch
    {
        >= 91 => 5,
        >= 81 => 4,
        >= 71 => 3,
        >= 61 => 2,
        >= 51 => 1,
        _ => 0
    };

}
