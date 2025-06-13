using AutoMapper;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.Features.KeyValues.DTOs;
using Cfo.Cats.Application.Features.KeyValues.Queries.PaginationQuery;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Documents;
using MassTransit;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportCaseWorkloadIntegrationEventConsumer(
    IUnitOfWork unitOfWork, 
    IExcelService excelService, 
    IUploadService uploadService, 
    IDomainEventDispatcher domainEventDispatcher) : IConsumer<ExportDocumentIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ExportDocumentIntegrationEvent> context)
    {
        if(context.Message.Key != DocumentTemplate.CaseWorkload.Name)
        {
            return;
        }

        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(context.Message.DocumentId);

        if(document is null)
        {
            return;
        }

        try
        {
            var request = JsonConvert.DeserializeObject<GetCaseWorkload.Query>(context.Message.SearchCriteria!)
                ?? throw new Exception();

            // Hack: call handler directly (skips Authorization pipeline, as we're outside of the HttpContext).
            var data = await new GetCaseWorkload.Handler(unitOfWork).Handle(request!, CancellationToken.None);

            if(data is not { Succeeded: true })
            {
                throw new Exception(data.ErrorMessage);
            }

            var results = await excelService.ExportAsync(data.Data ?? [],
                new Dictionary<string, Func<CaseSummaryDto, object?>>
                {
                    { "Tenant", item => item.TenantName },
                    { "Location", item => item.LocationName },
                    { "Status", item => item.GetEnrolmentStatus() },
                    { "User", item => item.UserName },
                    { "Count", item => item.Count },
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
