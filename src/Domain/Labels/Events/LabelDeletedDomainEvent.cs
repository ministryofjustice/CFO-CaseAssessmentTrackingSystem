using Cfo.Cats.Domain.Common.Events;

namespace Cfo.Cats.Domain.Labels.Events;

public sealed class LabelDeletedDomainEvent(Label entity) : DeletedDomainEvent<Label>(entity);