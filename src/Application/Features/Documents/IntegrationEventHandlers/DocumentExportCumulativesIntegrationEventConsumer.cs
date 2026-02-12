using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Common.Interfaces.Serialization;
using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.Features.ManagementInformation.Commands;
using Cfo.Cats.Application.Features.ManagementInformation.Providers;
using Cfo.Cats.Domain.Entities.Documents;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportCumulativesIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    ICumulativeProvider cumulativeProvider,
    IContractService contractService,
    ISerializer serializer,
    ICumulativeExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<DocumentExportCumulativesIntegrationEventConsumer> logger) : IHandleMessages<ExportDocumentIntegrationEvent>
{

    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.CumulativeFigures.Name)
        {
            logger.LogDebug($"Document type is not managed by this handler. Ignoring.");
            return;
        }

        var document = await unitOfWork.DbContext
            .GeneratedDocuments
            .FindAsync(context.DocumentId);

        if (document is null)
        {
            logger.LogWarning("Cannot find cumulatives document with id {DocumentId}", context.DocumentId);
            return;
        }

        if (document.SearchCriteriaUsed is null)
        {
            logger.LogWarning("Cumulatives document with id {DocumentId} has no search criteria", context.DocumentId);
            return;
        }

        try
        {
            // get the serialized query
            var command = serializer.Deserialize<ExportCumulativeFigures.Command>(document.SearchCriteriaUsed)
                          ?? throw new ArgumentException("Search criteria is not of type ExportCumulativeFigures.Command", nameof(document.SearchCriteriaUsed));

            string[] contracts = command.ContractId is not null
                ? [command.ContractId]
                : contractService.GetVisibleContracts(context.TenantId)
                    .Select(c => c.Id)
                    .ToArray();

            var january = new DateOnly(2025, 1, 1);
            var previousEnd = new DateOnly(command.EndDate.AddMonths(-1).Year, command.EndDate.AddMonths(-1).Month, DateTime.DaysInMonth(command.EndDate.AddMonths(-1).Year, command.EndDate.AddMonths(-1).Month));

            var thisMonthActuals = await cumulativeProvider.GetActuals(january, command.EndDate, contracts);
            var previousMonthActuals = await cumulativeProvider.GetActuals(january, previousEnd, contracts);
            var thisMonthsTargets = await cumulativeProvider.GetTargets(january, command.EndDate, contracts);
            var previousMonthsTargets = await cumulativeProvider.GetTargets(january, previousEnd, contracts);

            excelService.WithActuals(thisMonthActuals)
                .WithTargets(thisMonthsTargets)
                .WithLastMonthActuals(previousMonthActuals)
                .WithLastMonthTargets(previousMonthsTargets)
                .WithThisMonth(command.EndDate);

            var results = await excelService.ExportAsync();

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
                logger.LogError("Failed to upload cumulatives document {DocumentId}: {Errors}", context.DocumentId, string.Join(", ", result.Errors));
                document.WithStatus(DocumentStatus.Error);
            }

            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Error exporting cumulatives document {DocumentId}: {ErrorMessage}", context.DocumentId, ex.Message);
            document.WithStatus(DocumentStatus.Error);
            await unitOfWork.CommitTransactionAsync();
        }
    }
}
