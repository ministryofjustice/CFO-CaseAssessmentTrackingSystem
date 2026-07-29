using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.Features.PRIs.Commands;
using Cfo.Cats.Application.Features.PRIs.DTOs;
using Cfo.Cats.Application.Features.PRIs.Queries;
using Cfo.Cats.Domain.Entities.Documents;
using Newtonsoft.Json;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportActivePrisIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher,
    IMapper mapper,
    ILogger<DocumentExportActivePrisIntegrationEventConsumer> logger) : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.ActivePRIs.Name)
        {
            logger.LogDebug("Export document not supported by this handler");
            return;
        }

        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(context.DocumentId);

        if (document is null)
        {
            logger.LogError("Export active PRIs document event raised for a document that does not exist. ({DocumentId})", context.DocumentId);
            return;
        }

        try
        {
            var request = JsonConvert.DeserializeObject<ExportActivePRIs.ActivePRIsExportRequest>(context.SearchCriteria!)
                ?? throw new Exception("Failed to deserialise export request.");

            var stubUser = new UserProfile
            {
                UserName = "system",
                Email = "system@system",
                UserId = request.UserId ?? context.UserId,
                TenantId = context.TenantId
            };

            var query = new GetActivePRIsByUserId.Query
            {
                CurrentUser = stubUser,
                Keyword = request.Keyword,
                IncludeOutgoing = request.IncludeOutgoing,
                IncludeIncoming = request.IncludeIncoming,
                OrderBy = request.OrderBy ?? "Id",
                SortDirection = request.SortDirection ?? "Descending",
                PageNumber = 1,
                PageSize = int.MaxValue 
            };

            // Call handler directly (skips Authorization pipeline, as we're outside of the HttpContext)
            var data = await new GetActivePRIsByUserId.Handler(unitOfWork, mapper).Handle(query, CancellationToken.None);

            if (data is not { Succeeded: true, Data: not null })
            {
                throw new ApplicationException(data.ErrorMessage);
            }

            // Build lookup dictionary for user display names
            var userIds = data.Data.Items
                .SelectMany(pri => new[] { pri.CreatedBy, pri.AssignedTo })
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            var userDisplayNames = await unitOfWork.DbContext.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.DisplayName ?? u.UserName ?? u.Id);

            var results = await excelService.ExportAsync(
                data.Data.Items,
                new Dictionary<string, Func<PRIPaginationDto, object?>>
                {
                    { "Participant ID", item => item.ParticipantId },
                    { "Participant Name", item => item.ParticipantName },
                    { "Active", item => item.ParticipantIsActive ? "Yes" : "No" },
                    { "Custody Support Worker", item => item.CreatedBy != null && userDisplayNames.TryGetValue(item.CreatedBy, out var custodyName) ? custodyName : item.CreatedBy },
                    { "Community Support Worker", item => item.AssignedTo != null && userDisplayNames.TryGetValue(item.AssignedTo, out var communityName) ? communityName : item.AssignedTo },
                    { "Expected Release Region", item => item.ExpectedReleaseRegion?.Name },
                    { "Expected Release Date", item => item.ExpectedReleaseDate?.ToString("dd/MM/yyyy") },
                    { "Actual Release Date", item => item.ActualReleaseDate?.ToString("dd/MM/yyyy") ?? "Not set" }
                });

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
                logger.LogError("Failed to upload active PRIs document {DocumentId}: {Errors}", context.DocumentId, string.Join(", ", result.Errors));
                document.WithStatus(DocumentStatus.Error);
            }

            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting active PRIs document {DocumentId}: {ErrorMessage}", context.DocumentId, ex.Message);
            document.WithStatus(DocumentStatus.Error);
            await unitOfWork.CommitTransactionAsync();
        }
    }
}
