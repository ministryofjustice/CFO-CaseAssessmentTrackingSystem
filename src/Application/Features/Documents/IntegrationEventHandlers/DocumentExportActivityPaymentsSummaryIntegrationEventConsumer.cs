using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.Features.ManagementInformation;
using Cfo.Cats.Application.Features.Payments.DTOs;
using Cfo.Cats.Application.Features.Payments.Queries;
using Cfo.Cats.Domain.Entities.Documents;
using Newtonsoft.Json;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportActivityPaymentsSummaryIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher,
    ITargetsProvider targetsProvider,
    ILogger<DocumentExportActivityPaymentsSummaryIntegrationEventConsumer> logger) : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.ActivityPaymentsSummary.Name)
        {
            logger.LogDebug("Export document not supported by this handler");
            return;
        }

        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(context.DocumentId);

        if (document is null)
        {
            logger.LogError("Export activity payments summary document event raised for a document that does not exist. ({DocumentId})", context.DocumentId);
            return;
        }

        try
        {
            var request = JsonConvert.DeserializeObject<GetActivityPayments.Query>(context.SearchCriteria!)
                ?? throw new Exception();

            // Hack: call handler directly (skips Authorization pipeline, as we're outside of the HttpContext).
            var data = await new GetActivityPayments.Handler(unitOfWork, targetsProvider).Handle(request!, CancellationToken.None);

            if (data is not { Succeeded: true })
            {
                throw new Exception(data.ErrorMessage);
            }

            var results = await excelService.ExportAsync(data.Data?.ContractSummary ?? [],
                new Dictionary<string, Func<ActivityPaymentSummaryDto, object?>>
                {
                    { "Contract", item => item.Contract },
                    { "Support Work Ach", item => item.SupportWork },
                    { "Support Work Tgt", item => item.SupportWorkTarget },
                    { "Support Work %", item => item.SupportWorkPercentage },
                    { "Human Citizenship Ach", item => item.HumanCitizenship },
                    { "Human Citizenship Tgt", item => item.HumanCitizenshipTarget },
                    { "Human Citizenship %", item => item.HumanCitizenshipPercentage },
                    { "Community and Social Ach", item => item.CommunityAndSocial },
                    { "Community and Social Tgt", item => item.CommunityAndSocialTarget },
                    { "Community and Social %", item => item.CommunityAndSocialPercentage },
                    { "ISW Support Ach", item => item.ISWSupport },
                    { "ISW Support Tgt", item => item.ISWSupportTarget },
                    { "ISW Support %", item => item.ISWSupportPercentage }
                }
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
                logger.LogError("Failed to upload activity payments summary document {DocumentId}: {Errors}", context.DocumentId, string.Join(", ", result.Errors));
                document.WithStatus(DocumentStatus.Error);
            }

            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting activity payments summary document {DocumentId}: {ErrorMessage}", context.DocumentId, ex.Message);
            document.WithStatus(DocumentStatus.Error);
            await unitOfWork.CommitTransactionAsync();
        }
    }
}
