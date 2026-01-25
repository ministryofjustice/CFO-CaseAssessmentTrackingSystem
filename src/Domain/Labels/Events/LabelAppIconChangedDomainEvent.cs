using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Domain.Labels.Events;

public sealed class LabelAppIconChangedDomainEvent : DomainEvent
{
    public LabelId LabelId { get; }
    public AppIcon From { get; }
    public AppIcon To { get; }  

    public LabelAppIconChangedDomainEvent(LabelId labelId, AppIcon from, AppIcon to)
    {
        LabelId = labelId;
        From = from;
        To = to;
    }
}