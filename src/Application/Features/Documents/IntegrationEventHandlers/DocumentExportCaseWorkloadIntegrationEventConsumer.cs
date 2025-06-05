using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.IntegrationEvents;
using Cfo.Cats.Domain.Entities.Documents;
using MassTransit;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportCaseWorkloadIntegrationEventConsumer(
    IUnitOfWork unitOfWork, 
    IExcelService excelService, 
    IUploadService uploadService, 
    IDomainEventDispatcher domainEventDispatcher) : IConsumer<ExportCaseWorkloadIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ExportCaseWorkloadIntegrationEvent> context)
    {
        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(context.Message.DocumentId);

        if(document is null)
        {
            return;
        }

        try
        {
            var data = await unitOfWork.DbContext.Database
                .SqlQuery<CaseSummaryDto>(
                    $@"SELECT 
                    u.UserName,
                    u.TenantName,
                    p.EnrolmentStatus as EnrolmentStatusId, 
                    l.Name as LocationName,
                    COUNT(*) as [Count]
                FROM 
                    Participant.Participant as p
                INNER JOIN
                    [Configuration].Location as l ON p.CurrentLocationId = l.Id
                INNER JOIN
                    [Identity].[User] as u ON p.OwnerId = u.Id
                WHERE
                    u.TenantId LIKE {context.Message.TenantId}  
                GROUP BY 
                    u.UserName,
                    u.TenantName,
                    p.EnrolmentStatus,
                    l.Name"
                ).ToArrayAsync();

            var results = await excelService.ExportAsync(data,
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
