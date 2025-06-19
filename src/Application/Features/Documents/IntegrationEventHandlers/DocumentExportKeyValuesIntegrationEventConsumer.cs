using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.Features.KeyValues.DTOs;
using Cfo.Cats.Application.Features.KeyValues.Queries.PaginationQuery;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Domain.Entities.Documents;

using Newtonsoft.Json;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportKeyValuesIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IStringLocalizer<DocumentExportKeyValuesIntegrationEventConsumer> localizer,
    IUploadService uploadService,
    IMapper mapper,
    IDomainEventDispatcher domainEventDispatcher) : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.KeyValues.Name)
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
            var request = JsonConvert.DeserializeObject<KeyValuesWithPaginationQuery>(context.SearchCriteria!)
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