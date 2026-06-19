using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Domain.Entities.Documents;
using Newtonsoft.Json;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportUnassignedCasesDashboardIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<DocumentExportUnassignedCasesDashboardIntegrationEventConsumer> logger) : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.UnassignedCasesDashboard.Name)
        {
            logger.LogDebug("Export document not supported by this handler");
            return;
        }

        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(context.DocumentId);

        if (document is null)
        {
            logger.LogError("Unassigned cases dashboard export event raised for a document that does not exist. ({DocumentId})", context.DocumentId);
            return;
        }

        try
        {
            _ = JsonConvert.DeserializeObject<ExportUnassignedCasesDashboard.UnassignedCasesDashboardExportRequest>(context.SearchCriteria!)
                ?? throw new Exception("Failed to deserialise export request.");

            // TenantId must be set so the query's CurrentUser tenant scope filter works correctly.
            var stubUser = new UserProfile
            {
                UserName = "system",
                Email = "system@system",
                UserId = context.UserId,
                TenantId = context.TenantId
            };

            var query = new UnassignedCasesWithPagination.Query
            {
                CurrentUser = stubUser,
                PageNumber = 1,
                PageSize = 10000, // Export all
                IncludeTransferIn = true
            };

            var data = await new UnassignedCasesWithPagination.Handler(unitOfWork).Handle(query, CancellationToken.None);

            if (data is not { Succeeded: true, Data: not null })
            {
                throw new ApplicationException(data.ErrorMessage);
            }

            var rows = data.Data.Items.ToArray();

            var results = await excelService.ExportAsync(
                rows,
                new Dictionary<string, Func<UnassignedCaseDto, object?>>
                {
                    { "Participant ID", item => item.Id },
                    { "First Name", item => item.FirstName },
                    { "Last Name", item => item.LastName },
                    { "Enrolment Status", item => item.EnrolmentStatus.Name },
                    { "Consent Status", item => item.ConsentStatus.Name },
                    { "Current Location", item => item.CurrentLocation.Name },
                    { "Location Type", item => item.CurrentLocation.LocationType.Name },
                    { "Enrolment Location", item => item.EnrolmentLocation?.Name },
                    { "Tenant", item => item.TenantName },
                    { "Last Modified", item => item.LastModified },
                    { "Has Incoming Transfer", item => item.HasIncomingTransfer ? "Yes" : "No" }
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
                logger.LogError("Failed to upload unassigned cases dashboard document {DocumentId}: {Errors}", context.DocumentId, string.Join(", ", result.Errors));
                document.WithStatus(DocumentStatus.Error);
            }

            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting unassigned cases dashboard document {DocumentId}: {ErrorMessage}", context.DocumentId, ex.Message);
            document.WithStatus(DocumentStatus.Error);
            await unitOfWork.CommitTransactionAsync();
        }
    }
}
