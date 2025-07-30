namespace Cfo.Cats.Domain.Events;

public sealed class OutcomeQualityDipSampleParticipantScoredDomainEvent(Guid DipSampleId, string ReviewBy) : DomainEvent
{
    public Guid DipSampleId { get; set; } = DipSampleId;
    public string ReviewBy { get; set; } = ReviewBy;
}