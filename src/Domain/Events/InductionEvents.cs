using Cfo.Cats.Domain.Entities.Inductions;

namespace Cfo.Cats.Domain.Events;

public sealed class HubInductionCreatedEvent(HubInduction induction) : DomainEvent
{
    public HubInduction Item { get; } = induction;
}

public sealed class WingInductionCreatedEvent(WingInduction induction) : DomainEvent
{
    public WingInduction Item { get; } = induction;
}

public sealed class InductionPhaseCompletedEvent(InductionPhase phase) : DomainEvent
{
    public InductionPhase Item { get; } = phase;
}