namespace Cfo.Cats.Domain.Labels.Events;

public sealed class LabelRenamedDomainEvent(
    LabelId labelId, 
    string oldName, 
    string newName) : DomainEvent
{
    public LabelId LabelId { get; } = labelId;
    public string OldName { get; } = oldName;
    public string NewName { get; } = newName;
}
