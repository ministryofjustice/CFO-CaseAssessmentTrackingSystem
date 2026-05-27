using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.Commands;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Domain.Entities.Documents;
using Newtonsoft.Json;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportInitiativeObjectivesDashboardIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<DocumentExportInitiativeObjectivesDashboardIntegrationEventConsumer> logger) : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.InitiativeObjectivesDashboard.Name)
        {
            logger.LogDebug("Export document not supported by this handler");
            return;
        }

        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(context.DocumentId);

        if (document is null)
        {
            logger.LogError("Initiative objectives dashboard export event raised for a document that does not exist. ({DocumentId})", context.DocumentId);
            return;
        }

        try
        {
            var request = JsonConvert.DeserializeObject<ExportInitiativeObjectivesDashboard.InitiativeObjectivesDashboardExportRequest>(context.SearchCriteria!)
                ?? throw new Exception("Failed to deserialise export request.");

            var stubUser = new UserProfile
            {
                UserName = "system",
                Email = "system@system",
                UserId = context.UserId,
                TenantId = context.TenantId
            };

            var query = new GetInitiativeObjectivesDashboard.Query
            {
                CurrentUser = stubUser,
                UserId = request.UserId,
                TenantId = request.TenantId
            };

            var data = await new GetInitiativeObjectivesDashboard.Handler(unitOfWork).Handle(query, CancellationToken.None);

            if (data is not { Succeeded: true, Data: not null })
            {
                throw new ApplicationException(data.ErrorMessage ?? "Failed to fetch initiative objectives dashboard data");
            }

            var rows = data.Data
                .Where(r => string.IsNullOrWhiteSpace(request.InitiativeCode) || r.InitiativeCode == request.InitiativeCode)
                .Where(r => !request.ShowActiveOnly || !r.IsObjectiveCompleted)
                .ToArray();

            var results = await excelService.ExportAsync(
                rows,
                new Dictionary<string, Func<GetInitiativeObjectivesDashboard.InitiativeObjectiveRowDto, object?>>
                {
                    { "Participant", item => item.ParticipantName },
                    { "Participant Id", item => item.ParticipantId },
                    { "Assignee", item => item.OwnerDisplayName },
                    { "Assignee Tenant", item => item.OwnerTenantName },
                    { "Objective", item => item.ObjectiveDescription },
                    { "Created By", item => item.InitiativeObjectiveCreatedBy },
                    { "Initiative", item => item.InitiativeCode },
                    { "Initiative Description", item => item.InitiativeDescription },
                    { "Objective Status", item => item.IsObjectiveCompleted ? "Completed" : "Active" },
                    { "Completed Tasks", item => item.CompletedTasks },
                    { "Total Tasks", item => item.TotalTasks },
                    { "Outstanding Tasks", item => item.TotalTasks - item.CompletedTasks },
                    { "Activities", item => item.ActivityCount }
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
                logger.LogError("Failed to upload initiative objectives dashboard document {DocumentId}: {Errors}", context.DocumentId, string.Join(", ", result.Errors));
                document.WithStatus(DocumentStatus.Error);
            }

            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting initiative objectives dashboard document {DocumentId}: {ErrorMessage}", context.DocumentId, ex.Message);
            document.WithStatus(DocumentStatus.Error);
            await unitOfWork.CommitTransactionAsync();
        }
    }
}
