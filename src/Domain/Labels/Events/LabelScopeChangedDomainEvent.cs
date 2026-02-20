namespace Cfo.Cats.Domain.Labels.Events;

public sealed class LabelScopeChangedDomainEvent(LabelId labelId, LabelScope scope, LabelScope newScope) : DomainEvent
{
    public LabelId LabelId { get; } = labelId;
    public LabelScope FromScope {get;} = scope;
    public LabelScope ToScope {get;} = newScope;
}