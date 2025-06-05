namespace Cfo.Cats.Application.Features.Documents.IntegrationEvents;

public record ExportDocumentIntegrationEvent(Guid DocumentId, string Key, string UserId, string TenantId, string? SearchCriteria);