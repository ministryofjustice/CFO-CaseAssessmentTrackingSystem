using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Documents.EventHandlers;

public class AuditDocumentCreation(IUnitOfWork unitOfWork) : INotificationHandler<GeneratedDocumentCreatedDomainEvent>
{
    public async Task Handle(GeneratedDocumentCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await unitOfWork.DbContext.DocumentAuditTrails.AddAsync(DocumentAuditTrail.Create(notification.Entity.Id, notification.Entity.CreatedBy!, DocumentAuditTrailRequestType.DocumentGenerated, DateTime.UtcNow), cancellationToken);
    }
}
