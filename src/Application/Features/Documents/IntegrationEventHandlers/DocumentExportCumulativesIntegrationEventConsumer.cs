using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Common.Interfaces.Serialization;
using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.Features.ManagementInformation.Commands;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation.Providers;
using Cfo.Cats.Domain.Entities.Documents;
using MassTransit;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportCumulativesIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    ICumulativeProvider cumulativeProvider,
    IContractService contractService,
    ISerializer serializer,
    ICumulativeExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<DocumentExportCumulativesIntegrationEventConsumer> logger) : IConsumer<ExportDocumentIntegrationEvent>
{

    public async Task Consume(ConsumeContext<ExportDocumentIntegrationEvent> context)
    {
        if (context.Message.Key != DocumentTemplate.CumulativeFigures.Name)
        {
            logger.LogDebug($"Document type is not managed by this handler. Ignoring.");
            return;
        }

        var document = await unitOfWork.DbContext
            .GeneratedDocuments
            .FindAsync(context.Message.DocumentId);

        if (document is null)
        {
            logger.LogWarning("Cannot find document with id {DocumentId}", context.Message.DocumentId);
            return;
        }

        if (document.SearchCriteriaUsed is null)
        {
            logger.LogWarning("Document with id {DocumentId} has no search criteria", context.Message.DocumentId);
            return;
        }


        try
        {
            // get the serialized query
            var command = serializer.Deserialize<ExportCumulativeFigures.Command>(document.SearchCriteriaUsed)
                          ?? throw new ArgumentException("Search criteria is not of type ExportCumulativeFigures.Command", nameof(document.SearchCriteriaUsed));

            string[] contracts = command.ContractId is not null
                ? [command.ContractId]
                : contractService.GetVisibleContracts(context.Message.TenantId)
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

            var result = await uploadService.UploadAsync($"MyDocuments/{context.Message.UserId}", uploadRequest);

            document
                .WithStatus(DocumentStatus.Available)
                .SetURL(result);

            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Error exporting cumulatives");
            document.WithStatus(DocumentStatus.Error);
            await unitOfWork.CommitTransactionAsync();
        }
    }
}
