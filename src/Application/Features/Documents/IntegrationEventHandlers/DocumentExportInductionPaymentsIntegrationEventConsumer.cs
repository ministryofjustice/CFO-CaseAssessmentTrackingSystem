﻿using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.Features.ManagementInformation;
using Cfo.Cats.Application.Features.Payments.DTOs;
using Cfo.Cats.Application.Features.Payments.Queries;
using Cfo.Cats.Domain.Entities.Documents;
using Newtonsoft.Json;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportInductionPaymentsIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher,
    ITargetsProvider targetsProvider) : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.InductionPayments.Name)
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
            var request = JsonConvert.DeserializeObject<GetInductionPayments.Query>(context.SearchCriteria!)
                ?? throw new Exception();

            // Hack: call handler directly (skips Authorization pipeline, as we're outside of the HttpContext).
            var data = await new GetInductionPayments.Handler(unitOfWork, targetsProvider).Handle(request!, CancellationToken.None);

            if (data is not { Succeeded: true })
            {
                throw new Exception(data.ErrorMessage);
            }

            var results = await excelService.ExportAsync(data.Data?.Items ?? [],
                new Dictionary<string, Func<InductionPaymentDto, object?>>
                {
                    { "Contract", item => item.Contract },
                    { "Created", item => item.CreatedOn },
                    { "Commenced On", item => item.CommencedOn },
                    { "Approved", item => item.Approved },
                    { "Payment Period", item => item.PaymentPeriod },
                    { "Participant Id", item => item.ParticipantId },
                    { "Participant Name", item => item.ParticipantName },
                    { "Location Name", item => item.Location },
                    { "Location Type", item => item.LocationType },
                    { "Payable", item => item.EligibleForPayment },
                    { "Ineligibility Reason", item => item.IneligibilityReason }
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
