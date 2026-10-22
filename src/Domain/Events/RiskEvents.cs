namespace Cfo.Cats.Domain.Events;

public sealed class RiskInformationAddedDomainEvent(Entities.Participants.Risk risk) : DomainEvent
{
    public Entities.Participants.Risk Item { get; set; } = risk;
}

public sealed class RiskInformationReviewedDomainEvent(Entities.Participants.Risk risk) : DomainEvent
{
    public Entities.Participants.Risk Item { get; set; } = risk;
}

public sealed class RiskInformationCompletedDomainEvent(Entities.Participants.Risk risk) : DomainEvent
{
    public Entities.Participants.Risk Item { get; set; } = risk;
}
