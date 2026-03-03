namespace Cfo.Cats.Domain.ParticipantLabels.Events;

public sealed class ParticipantLabelClosedDomainEvent(ParticipantLabel label) : DomainEvent
{
    public ParticipantLabel Item { get; } = label;
}