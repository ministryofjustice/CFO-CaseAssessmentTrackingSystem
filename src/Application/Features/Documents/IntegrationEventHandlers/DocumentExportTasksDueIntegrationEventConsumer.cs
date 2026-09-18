using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.Features.PathwayPlans.Commands;
using Cfo.Cats.Application.Features.PathwayPlans.Queries;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;
using Newtonsoft.Json;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportTasksDueIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<DocumentExportTasksDueIntegrationEventConsumer> logger) : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.TasksDue.Name)
        {
            logger.LogDebug("Export document not supported by this handler");
            return;
        }

        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(context.DocumentId);

        if (document is null)
        {
            logger.LogError("Export tasks due document event raised for a document that does not exist. ({DocumentId})", context.DocumentId);
            return;
        }

        try
        {
            var request = JsonConvert.DeserializeObject<ExportTasksDue.TasksDueExportRequest>(context.SearchCriteria!)
                ?? throw new Exception("Failed to deserialise export request.");

            var stubUser = new UserProfile
            {
                UserName = "system",
                Email = "system@system",
                UserId = request.UserId ?? context.UserId,
                TenantId = request.TenantId ?? context.TenantId
            };

            var query = new TasksDueWithPagination.Query
            {
                CurrentUser = stubUser,
                OwnerId = request.OwnerId,
                Category = request.Category,
                Keyword = request.Keyword,
                OrderBy = request.OrderBy ?? "Due",
                SortDirection = request.SortDirection ?? "Ascending",
                PageNumber = 1,
                PageSize = int.MaxValue
            };

            // Call handler directly (skips Authorization pipeline, as we're outside the HttpContext)
            var data = await new TasksDueWithPagination.Handler(unitOfWork).Handle(query, CancellationToken.None);

            if (data is not { Succeeded: true, Data: not null })
            {
                throw new ApplicationException(data.ErrorMessage);
            }

            var results = await excelService.ExportAsync(
                data.Data.Items,
                new Dictionary<string, Func<TasksDueWithPagination.TaskDueDto, object?>>
                {
                    { "Status", item => item.Category.Humanize() },
                    { "Participant ID", item => item.ParticipantId },
                    { "Participant Name", item => item.ParticipantName },
                    { "Task", item => item.TaskDescription },
                    { "Objective", item => item.ObjectiveDescription },
                    { "Mandatory", item => item.IsMandatory ? "Yes" : "No" },
                    { "Due Date", item => item.Due.ToString("dd/MM/yyyy") },
                    { "Support Worker", item => item.OwnerName ?? "Unassigned" }
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
                logger.LogError("Failed to upload tasks due document {DocumentId}: {Errors}", context.DocumentId, string.Join(", ", result.Errors));
                document.WithStatus(DocumentStatus.Error);
            }

            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting tasks due document {DocumentId}: {ErrorMessage}", context.DocumentId, ex.Message);
            document.WithStatus(DocumentStatus.Error);
            await unitOfWork.CommitTransactionAsync();
        }
    }
}
