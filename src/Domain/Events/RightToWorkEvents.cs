using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Events;

public sealed class RightToWorkCreatedDomainEvent(RightToWork entity) : CreatedDomainEvent<RightToWork>(entity);