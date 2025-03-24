using Cfo.Cats.Domain.Entities.Inductions;

namespace Cfo.Cats.Domain.Events;

public sealed class HubInductionCreatedDomainEvent(HubInduction induction) : DomainEvent
{
    public HubInduction Item { get; } = induction;
}

public sealed class WingInductionCreatedDomainEvent(WingInduction induction) : DomainEvent
{
    public WingInduction Item { get; } = induction;
}

public sealed class InductionPhaseCompletedDomainEvent(Guid inductionId, InductionPhase phase) : DomainEvent
{
    public InductionPhase Item { get; } = phase;
    public Guid InductionId { get; } = inductionId;
}

public sealed class InductionPhaseAbandonedDomainEvent(Guid inductionId, InductionPhase phase) : DomainEvent
{
    public InductionPhase Item { get; } = phase;
    public Guid InductionId { get; } = inductionId;
}