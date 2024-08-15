using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Bios;

namespace Cfo.Cats.Domain.Events;

public sealed class BioCreatedDomainEvent(ParticipantBio entity)
    : CreatedDomainEvent<ParticipantBio>(entity);

