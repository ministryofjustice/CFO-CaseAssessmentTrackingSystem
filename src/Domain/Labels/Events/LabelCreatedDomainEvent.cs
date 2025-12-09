using Cfo.Cats.Domain.Common.Events;

namespace Cfo.Cats.Domain.Labels.Events;

public sealed class LabelCreatedDomainEvent(Label entity) : CreatedDomainEvent<Label>(entity);