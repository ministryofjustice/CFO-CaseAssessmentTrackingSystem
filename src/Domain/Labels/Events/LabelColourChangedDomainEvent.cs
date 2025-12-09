using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Domain.Labels.Events;

public sealed class LabelColourChangedDomainEvent(
    LabelId labelId, 
    AppColour fromColour, 
    AppColour toColour) : DomainEvent
{
    public LabelId LabelId { get; } = labelId;
    public AppColour FromColour { get; } = fromColour;
    public AppColour ToColour { get; } =  toColour;
}