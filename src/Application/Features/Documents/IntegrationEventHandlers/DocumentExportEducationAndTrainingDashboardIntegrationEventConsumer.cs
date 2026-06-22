using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.Commands;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Domain.Entities.Documents;
using Newtonsoft.Json;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportEducationAndTrainingDashboardIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<DocumentExportEducationAndTrainingDashboardIntegrationEventConsumer> logger) : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.EducationAndTrainingDashboard.Name)
        {
            logger.LogDebug("Export document not supported by this handler");
            return;
        }

        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(context.DocumentId);

        if (document is null)
        {
            logger.LogError("Education & Training dashboard export event raised for a document that does not exist. ({DocumentId})", context.DocumentId);
            return;
        }

        try
        {
            var request = JsonConvert.DeserializeObject<ExportEducationAndTrainingDashboard.EducationAndTrainingDashboardExportRequest>(context.SearchCriteria!)
                ?? throw new Exception("Failed to deserialise export request.");

            var stubUser = new UserProfile
            {
                UserName = "system",
                Email = "system@system",
                UserId = context.UserId,
                TenantId = context.TenantId
            };

            var query = new GetEducationAndTrainings.Query
            {
                CurrentUser = stubUser,
                UserId = request.UserId,
                TenantId = request.TenantId,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            var data = await new GetEducationAndTrainings.Handler(unitOfWork).Handle(query, CancellationToken.None);

            if (data is not { Succeeded: true, Data: not null })
            {
                throw new ApplicationException(data.ErrorMessage);
            }

            var rows = data.Data.Details.ToArray();

            var results = await excelService.ExportAsync(
                rows,
                new Dictionary<string, Func<GetEducationAndTrainings.LocationDetail, object?>>
                {
                    { "Location", item => item.LocationName },
                    { "Location Type", item => item.LocationType.Name },
                    { "Payable", item => item.Payable }
                });

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
                logger.LogError("Failed to upload education & training dashboard document {DocumentId}: {Errors}", context.DocumentId, string.Join(", ", result.Errors));
                document.WithStatus(DocumentStatus.Error);
            }

            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting education & training dashboard document {DocumentId}: {ErrorMessage}", context.DocumentId, ex.Message);
            document.WithStatus(DocumentStatus.Error);
            await unitOfWork.CommitTransactionAsync();
        }
    }
}

