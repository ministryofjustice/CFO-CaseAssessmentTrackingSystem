using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Documents;

namespace Cfo.Cats.Domain.Events;

public class DocumentCreatedDomainEvent(Document entity) : CreatedDomainEvent<Document>(entity);

public class DocumentUpdatedDomainEvent(Document entity) : UpdatedDomainEvent<Document>(entity);