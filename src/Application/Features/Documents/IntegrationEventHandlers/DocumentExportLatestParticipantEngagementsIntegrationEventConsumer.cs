using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Domain.Entities.Documents;
using Newtonsoft.Json;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportLatestParticipantEngagementsIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher) : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.ParticipantsLatestEngagement.Name)
        {
            return;
        }

        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(context.DocumentId);

        if (document is null)
        {
            return;
        }

        try
        {
            var request = JsonConvert.DeserializeObject<GetParticipantsLatestEngagement.Query>(context.SearchCriteria!)
                ?? throw new Exception();

            request.PageSize = int.MaxValue;

            // Hack: call handler directly (skips Authorization pipeline, as we're outside of the HttpContext).
            var data = await new GetParticipantsLatestEngagement.Handler(unitOfWork).Handle(request!, CancellationToken.None);

            if (data is not { Succeeded: true })
            {
                throw new Exception(data.ErrorMessage);
            }

            var results = await excelService.ExportAsync(data.Data?.Items ?? [],
                new Dictionary<string, Func<ParticipantEngagementDto, object?>>
                {
                    { "Participant Id", item => item.ParticipantId },
                    { "Participant Name", item => item.FullName },
                    { "Category", item => item.Category },
                    { "Description", item => item.Description },
                    { "Engaged at (Location)", item => item.EngagedAtLocationName },
                    { "Engaged at (Contract)", item => item.EngagedAtContractName },
                    { "Engaged on", item => item.EngagedOn?.ToShortDateString() },
                    { "Has Engaged Recently", item => item.HasEngagedRecently ? "Yes" : "No" },
                    { "Has Engaged", item => item.HasEngaged ? "Yes" : "No" },
                    { "Engaged With", item => item.EngagedWithDisplayName },
                    { "Engaged With (Tenant)", item => item.EngagedWithTenantName },
                    { "Support Worker", item => item.SupportWorkerDisplayName },
                    { "Current Location", item => item.CurrentLocationName },
                }
            );

            var uploadRequest = new UploadRequest(document.Title!, UploadType.Document, results);

            var result = await uploadService.UploadAsync($"MyDocuments/{context.UserId}", uploadRequest);

            document
                .WithStatus(DocumentStatus.Available)
                .SetURL(result);

            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            document.WithStatus(DocumentStatus.Error);
            await unitOfWork.CommitTransactionAsync();
        }

    }

}
