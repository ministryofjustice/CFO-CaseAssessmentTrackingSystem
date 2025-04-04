﻿using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Domain.Entities.Inductions;

public class InductionPhase(Guid id,int number, DateTime startDate, DateTime? completedDate, WingInductionPhaseStatus status, string? abandonJustification, WingInductionPhaseAbandonReason? abandonReason, string? completedBy) : ValueObject
{
    public Guid Id { get; private set; } = id;

    public int Number { get; private set; } = number;

    public DateTime StartDate { get; private set; } = startDate;
    
    public DateTime? CompletedDate { get; private set; } = completedDate;

    /// <summary>
    /// Status of the Wing Induction Phase
    /// </summary>
    public WingInductionPhaseStatus Status { get; private set; } = status;

    /// <summary>
    /// The justification for Abandoning Wing Induction Phase
    /// </summary>
    public string? AbandonJustification { get; private set; } = abandonJustification;

    /// <summary>
    /// The reason for Abandoning Wing Induction Phase
    /// </summary>
    public WingInductionPhaseAbandonReason? AbandonReason { get; private set; } = abandonReason;

    /// <summary>
    /// Who Abandoned the Wing Induction Phase
    /// </summary>
    public string? CompletedBy { get; private set; } = completedBy;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Number;
    }

    public void MarkAsCompleted(DateTime completionDate, string? completedBy)
    {
        CompletedDate = completionDate;
        Status = WingInductionPhaseStatus.Completed;
        CompletedBy = completedBy;
    }

    public void MarkAsAbandoned(DateTime abandonDate, string? abandonJustification, WingInductionPhaseAbandonReason? abandonReason, string abandonedBy)
    {
        CompletedDate = abandonDate;
        AbandonJustification= abandonJustification;
        AbandonReason = abandonReason;
        Status = WingInductionPhaseStatus.Abandoned;
        CompletedBy = abandonedBy;
    }
}