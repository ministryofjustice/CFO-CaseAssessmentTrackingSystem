using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.Features.KeyValues.DTOs;
using Cfo.Cats.Domain.Entities.Documents;
using MassTransit;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentKeyValuesIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IStringLocalizer<DocumentKeyValuesIntegrationEventConsumer> localizer,
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
#pragma warning disable CS8602
#pragma warning disable CS8604
            var data = await unitOfWork.DbContext.KeyValues.Where(x =>
                    x.Description.Contains(context.Message.SearchCriteria) || x.Value.Contains(context.Message.SearchCriteria) ||
                    x.Text.Contains(context.Message.SearchCriteria))
                .ProjectTo<KeyValueDto>(mapper.ConfigurationProvider)
                .ToListAsync();
#pragma warning restore CS8602
#pragma warning restore CS8604

            var results = await excelService.ExportAsync(data,
                new Dictionary<string, Func<KeyValueDto, object?>>
                {
                { localizer["Name"], item => item.Name },
                { localizer["Value"], item => item.Value },
                { localizer["Text"], item => item.Text },
                { localizer["Description"], item => item.Description }
                }, localizer["Data"]
            );

            var uploadRequest = new UploadRequest(document.Title!, UploadType.Document, results);

            var result = await uploadService.UploadAsync($"UserGeneratedDocuments/{context.Message.UserId}", uploadRequest);

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