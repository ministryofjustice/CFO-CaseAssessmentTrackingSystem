using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Documents;

namespace Cfo.Cats.Domain.Events;

public sealed class DocumentCreatedDomainEvent(Document entity) : CreatedDomainEvent<Document>(entity);

public sealed class DocumentUpdatedDomainEvent(Document entity) : UpdatedDomainEvent<Document>(entity);

public sealed class GeneratedDocumentCreatedDomainEvent(GeneratedDocument entity) : CreatedDomainEvent<GeneratedDocument>(entity);

public sealed class DocumentDownloadedDomainEvent(Document entity, DateTime occurredOn) : DomainEvent
{
    public Document Entity { get; set; } = entity;
    public DateTime OccurredOn { get; set; } = occurredOn;
}