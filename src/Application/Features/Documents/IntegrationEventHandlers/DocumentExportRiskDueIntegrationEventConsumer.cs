using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Domain.Entities.Documents;
using MassTransit;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportRiskDueIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher) : IConsumer<ExportDocumentIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ExportDocumentIntegrationEvent> context)
    {
        if (context.Message.Key != DocumentTemplate.RiskDue.Name)
        {
            return;
        }

        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(context.Message.DocumentId);

        if (document is null)
        {
            return;
        }

        try
        {
            var request = JsonConvert.DeserializeObject<GetRiskDueDashboard.Query>(context.Message.SearchCriteria!);

            // Hack: call handler directly (skips Authorization pipeline, as we're outside of the HttpContext).
            var data = await new GetRiskDueDashboard.Handler(unitOfWork).Handle(request!, CancellationToken.None);

            var results = await excelService.ExportAsync(data,
                new Dictionary<string, Func<RiskDueDto, object?>>
                {
                    { "Participant", item => item.ParticipantId },
                    { "First Name", item => item.FirstName },
                    { "Last Name", item => item.LastName },
                    { "Status", item => item.EnrolmentStatus },
                    { "Due Date", item => item.DueDate },
                    { "Review Reason", item => item.Reason },
                    { "Last Risk Update", item => item.LastRiskUpdate }
                }
            );

            var uploadRequest = new UploadRequest(document.Title!, UploadType.Document, results);

            var result = await uploadService.UploadAsync($"MyDocuments/{context.Message.UserId}", uploadRequest);

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
