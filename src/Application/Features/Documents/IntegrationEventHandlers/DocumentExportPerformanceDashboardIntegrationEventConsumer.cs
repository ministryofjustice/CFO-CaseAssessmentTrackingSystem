using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.Commands;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Domain.Entities.Documents;
using Newtonsoft.Json;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportPerformanceDashboardIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<DocumentExportPerformanceDashboardIntegrationEventConsumer> logger) : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.PerformanceDashboard.Name)
        {
            logger.LogDebug("Export document not supported by this handler");
            return;
        }

        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(context.DocumentId);

        if (document is null)
        {
            logger.LogError("Performance dashboard export event raised for a document that does not exist. ({DocumentId})", context.DocumentId);
            return;
        }

        try
        {
            var request = JsonConvert.DeserializeObject<ExportPerformanceDashboard.PerformanceDashboardExportRequest>(context.SearchCriteria!)
                ?? throw new Exception("Failed to deserialise export request.");

            // TenantId must be set so the query's CurrentUser tenant scope filter works correctly.
            var stubUser = new UserProfile { UserName = "system", Email = "system@system", UserId = context.UserId, TenantId = context.TenantId };

            var sheets = new (string SheetName, byte[] Data)[]
            {
                await BuildArchivedCasesSheet(request, stubUser),
            };

            var merged = await excelService.MergeSheetsAsync(sheets);

            var uploadRequest = new UploadRequest(document.Title!, UploadType.Document, merged);
            var result = await uploadService.UploadAsync($"MyDocuments/{context.UserId}", uploadRequest);

            if (result.Succeeded)
            {
                document.WithStatus(DocumentStatus.Available).SetURL(result);
            }
            else
            {
                logger.LogError("Failed to upload performance dashboard document {DocumentId}: {Errors}", context.DocumentId, string.Join(", ", result.Errors));
                document.WithStatus(DocumentStatus.Error);
            }

            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting performance dashboard document {DocumentId}: {ErrorMessage}", context.DocumentId, ex.Message);
            document.WithStatus(DocumentStatus.Error);
            await unitOfWork.CommitTransactionAsync();
        }
    }

    private async Task<(string SheetName, byte[] Data)> BuildArchivedCasesSheet(
        ExportPerformanceDashboard.PerformanceDashboardExportRequest request, UserProfile stubUser)
    {
        var query = new GetArchivedCasesByTenantAndReason.Query
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TenantId = request.TenantId,
            CurrentUser = stubUser
        };

        var data = await new GetArchivedCasesByTenantAndReason.Handler(unitOfWork).Handle(query, CancellationToken.None);

        if (data is not { Succeeded: true })
        {
            throw new Exception(data.ErrorMessage);
        }

        var sheet = await excelService.ExportAsync(data.Data!.TabularData,
            new Dictionary<string, Func<GetArchivedCasesByTenantAndReason.ArchivedCasesTabularData, object?>>
            {
                { "Tenant",      r => r.Tenant },
                { "Reason",      r => r.Reason },
                { "Participant", r => r.ParticipantId },
                { "First Name",  r => r.FirstName },
                { "Last Name",   r => r.LastName },
                { "Location",    r => r.Location },
                { "Created",     r => r.Created.ToShortDateString() },
                { "From",        r => r.From.ToShortDateString() },
                { "To",          r => r.To?.ToShortDateString() },
                { "Created By",  r => r.CreatedBy },
            });

        return ("Archived Cases", sheet);
    }
}
