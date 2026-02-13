using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.Features.KeyValues.DTOs;
using Cfo.Cats.Application.Features.KeyValues.Queries.PaginationQuery;
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
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<DocumentExportKeyValuesIntegrationEventConsumer> logger) : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.KeyValues.Name)
        {
            logger.LogDebug("Export document not supported by this handler");
            return;
        }

        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(context.DocumentId);

        if (document is null)
        {
            logger.LogError("Export key values document event raised for a document that does not exist. ({DocumentId})", context.DocumentId);
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

            if (result.Succeeded)
            {
                document
                    .WithStatus(DocumentStatus.Available)
                    .SetURL(result);
            }
            else
            {
                logger.LogError("Failed to upload key values document {DocumentId}: {Errors}", context.DocumentId, string.Join(", ", result.Errors));
                document.WithStatus(DocumentStatus.Error);
            }

            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting key values document {DocumentId}: {ErrorMessage}", context.DocumentId, ex.Message);
            document.WithStatus(DocumentStatus.Error);
            await unitOfWork.CommitTransactionAsync();
        }

    }
}