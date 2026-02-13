using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Domain.Entities.Documents;
using Newtonsoft.Json;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportCaseWorkloadIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<DocumentExportCaseWorkloadIntegrationEventConsumer> logger) : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.CaseWorkload.Name)
        {
            logger.LogDebug("Export document not supported by this handler");
            return;
        }

        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(context.DocumentId);

        if (document is null)
        {
            logger.LogError("Export case workload document event raised for a document that does not exist. ({DocumentId})", context.DocumentId);
            return;
        }

        try
        {
            var request = JsonConvert.DeserializeObject<GetCaseWorkload.Query>(context.SearchCriteria!)
                ?? throw new Exception();

            // Hack: call handler directly (skips Authorization pipeline, as we're outside of the HttpContext).
            var data = await new GetCaseWorkload.Handler(unitOfWork).Handle(request!, CancellationToken.None);

            if (data is not { Succeeded: true })
            {
                throw new Exception(data.ErrorMessage);
            }

            var results = await excelService.ExportAsync(data.Data ?? [],
                new Dictionary<string, Func<CaseSummaryDto, object?>>
                {
                    { "Tenant", item => item.TenantName },
                    { "Location", item => item.LocationName },
                    { "Status", item => item.GetEnrolmentStatus() },
                    { "User", item => item.UserName },
                    { "Count", item => item.Count },
                }
            );

            var uploadRequest = new UploadRequest(document.Title!, UploadType.Document, results);

            var result = await uploadService.UploadAsync($"MyDocuments/{context.UserId}", uploadRequest);

            if (result.Succeeded)
            {
                document
                    .WithStatus(DocumentStatus.Available)
                    .SetURL(result);
            }
            else
            {
                logger.LogError("Failed to upload case workload document {DocumentId}: {Errors}", context.DocumentId, string.Join(", ", result.Errors));
                document.WithStatus(DocumentStatus.Error);
            }

            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting case workload document {DocumentId}: {ErrorMessage}", context.DocumentId, ex.Message);
            document.WithStatus(DocumentStatus.Error);
            await unitOfWork.CommitTransactionAsync();
        }

    }

}
