using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Documents.EventHandlers;

public class GenerateDocumentOnCreation(IUnitOfWork unitOfWork) : INotificationHandler<GeneratedDocumentCreatedDomainEvent>
{
    public async Task Handle(GeneratedDocumentCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var document = notification.Entity;

        await unitOfWork.DbContext.InsertOutboxMessage(new ExportDocumentIntegrationEvent(
            document.Id,
            document.Template.Name, 
            document.CreatedBy!, 
            document.TenantId!, 
            document.SearchCriteriaUsed));
    }
}
