namespace Cfo.Cats.Domain.Events;

public sealed class OutcomeQualityDipSampleParticipantScoredDomainEvent(
    Guid dipSampleId,
    string reviewBy,
    bool isCompliant) : DomainEvent
{
    public Guid DipSampleId { get; set; } = dipSampleId;
    public string ReviewBy { get; set; } = reviewBy;
    public bool IsCompliant { get; set; } = isCompliant;
}

public sealed class OutcomeQualityDipSampleVerifyingDomainEvent(
    Guid dipSampleId,
    string userId,
    DateTime occurredOn) : DomainEvent
{
    public Guid DipSampleId { get; } = dipSampleId;
    public string UserId { get; } = userId;
    public DateTime OccurredOn { get; } = occurredOn;
}

public sealed class OutcomeQualityDipSampleFinalisedDomainEvent(
    Guid dipSampleId,
    string userId,
    DateTime occurredOn) : DomainEvent
{
    public Guid DipSampleId { get; } = dipSampleId;
    public string UserId { get; } = userId;
    public DateTime OccurredOn { get; } = occurredOn;
}