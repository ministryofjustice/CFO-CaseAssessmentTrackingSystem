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