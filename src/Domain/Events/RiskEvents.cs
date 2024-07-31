using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Events;

public sealed class RiskInformationAddedDomainEvent(Risk risk) : DomainEvent
{
    public Risk Item { get; set; } = risk;
}
