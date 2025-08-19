using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Queries;
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
    IDomainEventDispatcher domainEventDispatcher) : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.Participants.Name)
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
            var request = JsonConvert.DeserializeObject<ParticipantsWithPagination.Query>(context.SearchCriteria!) 
                ?? throw new Exception();

            request.PageSize = int.MaxValue;

            // Hack: call handler directly (skips Authorization pipeline, as we're outside of the HttpContext).
            var data = await new ParticipantsWithPagination.Handler(unitOfWork).Handle(request!, CancellationToken.None);

            var results = await excelService.ExportAsync(data?.Data?.Items ?? [],
                new Dictionary<string, Func<ParticipantPaginationDto, object?>>
                {
                    { "Id", item => item.Id },
                    { "Participant", item => item.ParticipantName },
                    { "Status", item => item.EnrolmentStatus },
                    { "Consent", item => item.ConsentStatus },
                    { "Location", item => item.CurrentLocation },
                    { "Enrolled At", item => item.EnrolmentLocation },
                    { "Assignee", item => item.Owner },
                    { "Risk Due", item => item.RiskDue },
                    { "Risk Due Reason", item => item.RiskDueReason.Name }
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
