namespace Cfo.Cats.Application.Features.Dashboard.IntegrationEvents;

public record ExportCaseWorkloadIntegrationEvent(Guid DocumentId, string UserId, string TenantId, string? SearchCriteria);