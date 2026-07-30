using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Domain.Entities.Documents;
using Newtonsoft.Json;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportEngagementsByLocationIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<DocumentExportEngagementsByLocationIntegrationEventConsumer> logger) : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.EngagementsByLocation.Name)
        {
            logger.LogDebug("Export document not supported by this handler");
            return;
        }

        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(context.DocumentId);

        if (document is null)
        {
            logger.LogError("Export engagements by location document event raised for a document that does not exist. ({DocumentId})", context.DocumentId);
            return;
        }

        try
        {
            var request = JsonConvert.DeserializeObject<GetEngagementsByLocation.Query>(context.SearchCriteria!)
                ?? throw new Exception();

            request.PageSize = int.MaxValue;

            // Hack: call handler directly (skips Authorization pipeline, as we're outside of the HttpContext).
            var data = await new GetEngagementsByLocation.Handler(unitOfWork).Handle(request, CancellationToken.None);

            if (data is not { Succeeded: true })
            {
                throw new Exception(data.ErrorMessage);
            }

            var results = await excelService.ExportAsync(data.Data?.Details.Items ?? [],
                new Dictionary<string, Func<ParticipantEngagementDto, object?>>
                {
                    { "Participant Id", item => item.ParticipantId },
                    { "Participant Name", item => item.FullName },
                    { "Category", item => item.Category },
                    { "Description", item => item.Description },
                    { "Engaged at (Location)", item => item.EngagedAtLocationName },
                    { "Engaged at (Contract)", item => item.EngagedAtContractName },
                    { "Engaged With", item => item.EngagedWithDisplayName },
                    { "Engaged With (Tenant)", item => item.EngagedWithTenantName },
                    { "Support Worker", item => item.SupportWorkerDisplayName },
                    { "Current Location", item => item.CurrentLocationName },
                    { "Engaged on", item => item.EngagedOn },
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
                logger.LogError("Failed to upload engagements by location document {DocumentId}: {Errors}", context.DocumentId, string.Join(", ", result.Errors));
                document.WithStatus(DocumentStatus.Error);
            }

            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting engagements by location document {DocumentId}: {ErrorMessage}", context.DocumentId, ex.Message);
            document.WithStatus(DocumentStatus.Error);
            await unitOfWork.CommitTransactionAsync();
        }
    }
}
