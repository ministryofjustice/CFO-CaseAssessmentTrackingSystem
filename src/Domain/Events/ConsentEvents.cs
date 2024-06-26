using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Events;


public sealed class ConsentCreatedDomainEvent(Consent entity) : CreatedDomainEvent<Consent>(entity);