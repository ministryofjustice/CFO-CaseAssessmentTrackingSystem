using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportUsersIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher,
    UserManager<ApplicationUser> userManager,
    ILogger<DocumentExportUsersIntegrationEventConsumer> logger) : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.Users.Name)
        {
            logger.LogDebug("Export document not supported by this handler");
            return;
        }

        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(context.DocumentId);

        if (document is null)
        {
            logger.LogError("Export users document event raised for a document that does not exist. ({DocumentId})", context.DocumentId);
            return;
        }

        try
        {
            var request = JsonConvert.DeserializeObject<Identity.Commands.ExportUsers.Command>(context.SearchCriteria!)
                ?? new Identity.Commands.ExportUsers.Command();

            // Build the query
            var query = userManager.Users
                .Where(u => u.TenantId!.StartsWith(context.TenantId));

            // Apply filters
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                var searchLower = request.SearchString.ToLower();
                query = query.Where(u =>
                    u.UserName!.Contains(searchLower) ||
                    u.Email!.Contains(searchLower) ||
                    u.DisplayName!.Contains(searchLower) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchLower)));
            }

            if (!string.IsNullOrEmpty(request.Role))
            {
                query = query.Where(u => u.UserRoles.Any(ur => ur.Role.Name == request.Role));
            }

            if (!string.IsNullOrEmpty(request.TenantId))
            {
                query = query.Where(u => u.TenantId == request.TenantId);
            }

            var users = await query
                .OrderBy(u => u.DisplayName)
                .ToListAsync();

            var dataToColumnMapper = new Dictionary<string, Func<ApplicationUser, object?>>
            {
                { "Name", user => user.DisplayName },
                { "Email", user => user.Email },
                { "Status", user => user.IsActive ? "Active" : "Inactive" },
                { "Lockout Status", user => user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow ? "Locked Out" : "Not Locked" },
                { "Lockout End Date", user => user.LockoutEnd },
                { "Last Login", user => user.LastLogin },
                { "Tenant", user => user.TenantName },
                { "Created Date", user => user.Created }
            };

            var results = await excelService.ExportAsync(users, dataToColumnMapper);

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
                logger.LogError("Failed to upload users document {DocumentId}: {Errors}", context.DocumentId, string.Join(", ", result.Errors));
                document.WithStatus(DocumentStatus.Error);
            }

            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting users document {DocumentId}: {ErrorMessage}", context.DocumentId, ex.Message);
            document.WithStatus(DocumentStatus.Error);
            await unitOfWork.CommitTransactionAsync();
        }
    }
}
