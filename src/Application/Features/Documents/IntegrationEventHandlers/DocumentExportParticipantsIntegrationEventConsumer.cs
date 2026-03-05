using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Domain.Entities.Documents;
using Newtonsoft.Json;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportParticipantsIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<DocumentExportParticipantsIntegrationEventConsumer> logger) : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.Participants.Name)
        {
            logger.LogDebug("Export document not supported by this handler");
            return;
        }

        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(context.DocumentId);

        if (document is null)
        {
            logger.LogError("Export participants document event raised for a document that does not exist. ({DocumentId})", context.DocumentId);
            return;
        }

        try
        {
            var request = JsonConvert.DeserializeObject<ParticipantsWithPagination.Query>(context.SearchCriteria!)
                ?? throw new Exception();

            request.PageSize = int.MaxValue;

            // Hack: call handler directly (skips Authorization pipeline, as we're outside of the HttpContext).
            var data = await new ParticipantsWithPagination.Handler(unitOfWork).Handle(request!, CancellationToken.None);

            var dataToColumnMapper = new Dictionary<string, Func<ParticipantPaginationDto, object?>>
            {
                { "Id", item => item.Id },
                { "Participant", item => item.ParticipantName },
                { "Status", item => item.EnrolmentStatus },
                { "Consent", item => item.ConsentStatus },
                { "Location", item => item.CurrentLocation.Name },
                { "Enrolled At", item => item.EnrolmentLocation?.Name },
                { "Assignee", item => item.Owner },
                { "Risk Due", item => item.RiskDue },
                { "Risk Due Reason", item => item.RiskDueReason.Name },
            };
            
            if(!data.Succeeded || data?.Data is null)
            {
                throw new ApplicationException("Failed to fetch participant data");
            }
            
            // get a list of labels from all the participants
            var allLabels = data.Data.Items.SelectMany(p => p.Labels)
                .GroupBy(label => label.Name)
                .Select(labelGroup => labelGroup.Key);

            foreach (var label in allLabels)
            {
                dataToColumnMapper.Add(label, dto => dto.Labels.Any(l => l.Name == label) ? "Y" : "N");
            }
            
            var results = await excelService.ExportAsync(data?.Data?.Items ?? [],
                dataToColumnMapper
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
                logger.LogError("Failed to upload participants document {DocumentId}: {Errors}", context.DocumentId, string.Join(", ", result.Errors));
                document.WithStatus(DocumentStatus.Error);
            }

            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting participants document {DocumentId}: {ErrorMessage}", context.DocumentId, ex.Message);
            document.WithStatus(DocumentStatus.Error);
            await unitOfWork.CommitTransactionAsync();
        }
    }
}
