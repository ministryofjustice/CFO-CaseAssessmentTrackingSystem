using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class OutcomeQualityDipSample
{
#pragma warning disable CS8618 
    internal OutcomeQualityDipSample() { /* this is for EF Core */}
#pragma warning restore CS8618

    public static OutcomeQualityDipSample Create(string contractId, DateTime periodFrom, DateTime periodTo) => new()
    {
        Id = Guid.CreateVersion7(),
        ContractId = contractId,
        CreatedOn = DateTime.UtcNow,
        Status = DipSampleStatus.InProgress,
        PeriodFrom = periodFrom,
        PeriodTo = periodTo
    };

    public OutcomeQualityDipSample Complete(double scoreAvg, string completedBy)
    {
        if (CompletedOn.HasValue is false)
        {
            CompletedOn = DateTime.UtcNow;
            CompletedBy = completedBy;
            Status = DipSampleStatus.Completed;
            ScoreAvg = scoreAvg;
        }

        return this;
    }

    public Guid Id { get; private set; }
    public string ContractId { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime PeriodFrom { get; private set; }
    public DateTime PeriodTo { get; private set; }
    public DateTime? CompletedOn { get; private set; }
    public string? CompletedBy { get; private set; }
    public double? ScoreAvg { get; private set; }
    public DipSampleStatus Status { get; private set; }
}
