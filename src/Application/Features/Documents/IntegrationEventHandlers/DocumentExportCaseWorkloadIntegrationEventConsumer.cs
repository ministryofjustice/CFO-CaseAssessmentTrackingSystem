using AutoMapper;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.Features.KeyValues.DTOs;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Documents;
using MassTransit;

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
            var searchCriteria = context.Message.SearchCriteria ?? string.Empty;
            var tenantPrefix = context.Message.TenantId;

            var statuses = EnrolmentStatus.List
                .Where(e => e.Name.Contains(searchCriteria))
                .Select(e => e.Value)
                .ToList();

#pragma warning disable CS8602
            var query =
                from p in unitOfWork.DbContext.Participants
                where p.Owner.TenantId.StartsWith(tenantPrefix)
                   && (
                       p.Owner.UserName.Contains(searchCriteria) ||
                       p.Owner.DisplayName.Contains(searchCriteria) ||
                       p.Owner.TenantName.Contains(searchCriteria) ||
                       statuses.Contains(p.EnrolmentStatus)
                      )
                group p by new
                {
                    p.Owner.UserName,
                    p.Owner.TenantName,
                    p.EnrolmentStatus,
                    LocationName = p.CurrentLocation.Name
                } into g
                select new CaseSummaryDto
                {
                    UserName = g.Key.UserName,
                    TenantName = g.Key.TenantName,
                    EnrolmentStatusId = g.Key.EnrolmentStatus,
                    LocationName = g.Key.LocationName,
                    Count = g.Count()
                };
#pragma warning restore CS8602

            var data = await query.ToListAsync();

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
