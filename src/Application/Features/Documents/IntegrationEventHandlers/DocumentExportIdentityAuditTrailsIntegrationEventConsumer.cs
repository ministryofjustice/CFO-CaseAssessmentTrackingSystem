using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.Features.Identity.Commands;
using Cfo.Cats.Application.Features.Identity.Specifications;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;
using Newtonsoft.Json;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportIdentityAuditTrailsIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<DocumentExportIdentityAuditTrailsIntegrationEventConsumer> logger) : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.IdentityAuditTrails.Name)
        {
            logger.LogDebug("Export document not supported by this handler");
            return;
        }

        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(context.DocumentId);
        if (document is null)
        {
            logger.LogError("Export user audit document event raised for a document that does not exist. ({DocumentId})", context.DocumentId);
            return;
        }

        try
        {
            var command = JsonConvert.DeserializeObject<ExportIdentityAuditTrails.Command>(context.SearchCriteria!)
                ?? throw new JsonSerializationException("Unable to deserialize user audit export request.");
            var request = command.Request;
            var auditTrails = unitOfWork.DbContext.IdentityAuditTrails.AsQueryable();

            if (request.IdentityActionType is not null)
            {
                auditTrails = auditTrails.Where(auditTrail => auditTrail.ActionType == request.IdentityActionType);
            }

            if (request.UserName is not null)
            {
                auditTrails = auditTrails.Where(auditTrail => auditTrail.UserName == request.UserName);
            }

            if (request.ListView == IdentityAuditTrailListView.CreatedToday)
            {
                auditTrails = auditTrails.Where(auditTrail => auditTrail.DateTime.Date == DateTime.Now.Date);
            }
            else if (request.ListView == IdentityAuditTrailListView.Last30days)
            {
                auditTrails = auditTrails.Where(auditTrail => auditTrail.DateTime >= DateTime.Now.ToUniversalTime().Date.AddDays(-30));
            }

            var results = await auditTrails
                .OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ToListAsync();

            var dataToColumnMapper = new Dictionary<string, Func<IdentityAuditTrail, object?>>
            {
                { "Date Time", auditTrail => auditTrail.DateTime },
                { "Action Type", auditTrail => auditTrail.ActionType.Humanize() },
                { "User Name", auditTrail => auditTrail.UserName },
                { "Performed By", auditTrail => auditTrail.PerformedBy },
                { "Remote IP Address", auditTrail => auditTrail.IpAddress }
            };

            var workbook = await excelService.ExportAsync(results, dataToColumnMapper);
            var uploadRequest = new UploadRequest(document.Title!, UploadType.Document, workbook);
            var uploadResult = await uploadService.UploadAsync($"MyDocuments/{context.UserId}", uploadRequest);

            if (uploadResult.Succeeded)
            {
                document.WithStatus(DocumentStatus.Available).SetURL(uploadResult);
            }
            else
            {
                logger.LogError("Failed to upload user audit document {DocumentId}: {Errors}", context.DocumentId, string.Join(", ", uploadResult.Errors));
                document.WithStatus(DocumentStatus.Error);
            }

            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error exporting user audit document {DocumentId}: {ErrorMessage}", context.DocumentId, exception.Message);
            document.WithStatus(DocumentStatus.Error);
            await unitOfWork.CommitTransactionAsync();
        }
    }
}