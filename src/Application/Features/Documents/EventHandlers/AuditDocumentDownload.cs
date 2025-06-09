using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Documents.EventHandlers;

public class AuditDocumentDownload(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : INotificationHandler<DocumentDownloadedDomainEvent>
{
    public async Task Handle(DocumentDownloadedDomainEvent notification, CancellationToken cancellationToken)
    {
        if(notification.Entity is not GeneratedDocument)
        {
            return;
        }

        await unitOfWork.DbContext.DocumentAuditTrails.AddAsync(
            DocumentAuditTrail.Create(notification.Entity.Id, currentUserService.UserId!, DocumentAuditTrailRequestType.DocumentDownloaded, notification.OccurredOn),
            cancellationToken);
    }
}
