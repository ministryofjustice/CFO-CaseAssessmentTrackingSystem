using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Events;

public sealed class RiskInformationAddedDomainEvent(Risk risk) : DomainEvent
{
    public Risk Item { get; set; } = risk;
}

public sealed class RiskInformationReviewedDomainEvent(Risk risk) : DomainEvent
{
    public Risk Item { get; set; } = risk;
}

public sealed class RiskInformationCompletedDomainEvent(Risk risk) : DomainEvent
{
    public Risk Item { get; set; } = risk;
}
