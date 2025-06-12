using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.Features.ManagementInformation;
using Cfo.Cats.Application.Features.Payments.DTOs;
using Cfo.Cats.Application.Features.Payments.Queries;
using Cfo.Cats.Domain.Entities.Documents;
using MassTransit;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportEnrolmentPaymentsIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher,
    ITargetsProvider targetsProvider) : IConsumer<ExportDocumentIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ExportDocumentIntegrationEvent> context)
    {
        if (context.Message.Key != DocumentTemplate.EnrolmentPayments.Name)
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
            var request = JsonConvert.DeserializeObject<GetEnrolmentPayments.Query>(context.Message.SearchCriteria!)
                ?? throw new Exception();

            // Hack: call handler directly (skips Authorization pipeline, as we're outside of the HttpContext).
            var data = await new GetEnrolmentPayments.Handler(unitOfWork, targetsProvider).Handle(request!, CancellationToken.None);

            if (data is not { Succeeded: true })
            {
                throw new Exception(data.ErrorMessage);
            }

            var results = await excelService.ExportAsync(data.Data?.Items ?? [],
                new Dictionary<string, Func<EnrolmentPaymentDto, object?>>
                {
                    { "Contract", item => item.Contract },
                    { "Created", item => item.CreatedOn },
                    { "Approved", item => item.Approved },
                    { "Submission To Authority", item => item.SubmissionToAuthority },
                    { "Participant Id", item => item.ParticipantId },
                    { "Participant Name", item => item.ParticipantName },
                    { "Location Name", item => item.Location },
                    { "Location Type", item => item.LocationType },
                    { "Payable", item => item.EligibleForPayment },
                    { "Ineligibility Reason", item => item.IneligibilityReason }
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
