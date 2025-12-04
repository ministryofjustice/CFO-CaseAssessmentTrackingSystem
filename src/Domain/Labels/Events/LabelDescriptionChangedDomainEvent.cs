namespace Cfo.Cats.Domain.Labels.Events;

public sealed class LabelDescriptionChangedDomainEvent(
    LabelId labelId, 
    string fromDescription, 
    string newDescription) : DomainEvent
{
    public LabelId LabelId { get; } = labelId;
    public string FromDescription { get; } = fromDescription;
    public string NewDescription { get; } = newDescription;
}
