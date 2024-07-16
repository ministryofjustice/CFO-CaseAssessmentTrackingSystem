using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Bios;

namespace Cfo.Cats.Domain.Events;

public sealed class BioCreatedDomainEvent(Bio entity)
    : CreatedDomainEvent<Bio>(entity);

