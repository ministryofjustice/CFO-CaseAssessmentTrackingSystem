using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.Features.KeyValues.DTOs;
using Cfo.Cats.Application.Features.KeyValues.Queries.PaginationQuery;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Domain.Entities.Documents;
using MassTransit;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportKeyValuesIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IStringLocalizer<DocumentExportKeyValuesIntegrationEventConsumer> localizer,
    IUploadService uploadService,
    IMapper mapper,
    IDomainEventDispatcher domainEventDispatcher) : IConsumer<ExportDocumentIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ExportDocumentIntegrationEvent> context)
    {
        if (context.Message.Key != DocumentTemplate.KeyValues.Name)
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
            var request = JsonConvert.DeserializeObject<KeyValuesWithPaginationQuery>(context.Message.SearchCriteria!)
                ?? throw new Exception();

            request.PageSize = int.MaxValue;

            // Hack: call handler directly (skips Authorization pipeline, as we're outside of the HttpContext).
            var data = await new KeyValuesQueryHandler(unitOfWork, mapper).Handle(request!, CancellationToken.None);

            var results = await excelService.ExportAsync(data.Items,
                new Dictionary<string, Func<KeyValueDto, object?>>
                {
                { localizer["Name"], item => item.Name },
                { localizer["Value"], item => item.Value },
                { localizer["Text"], item => item.Text },
                { localizer["Description"], item => item.Description }
                }, localizer["Data"]
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