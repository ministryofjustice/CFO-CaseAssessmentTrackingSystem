namespace Cfo.Cats.Domain.Events;

public sealed class OutcomeQualityDipSampleParticipantScored(Guid DipSampleId, string ReviewBy) : DomainEvent
{
    public Guid DipSampleId { get; set; } = DipSampleId;
    public string ReviewBy { get; set; } = ReviewBy;
}