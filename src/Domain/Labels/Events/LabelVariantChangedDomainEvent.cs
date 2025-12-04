using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Domain.Labels.Events;

public sealed class LabelVariantChangedDomainEvent(
    LabelId labelId,
    AppVariant fromVariant,
    AppVariant toVariant) : DomainEvent
{
    public LabelId LabelId { get; } = labelId;
    public AppVariant FromVariant { get; } = fromVariant;
    public AppVariant ToVariant { get; } = toVariant;
}