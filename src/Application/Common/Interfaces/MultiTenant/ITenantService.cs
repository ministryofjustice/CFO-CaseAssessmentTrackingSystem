using Cfo.Cats.Application.Features.Tenants.DTOs;

namespace Cfo.Cats.Application.Common.Interfaces.MultiTenant;

public interface ITenantService
{
    IReadOnlyList<TenantDto> DataSource { get; }
    event Action? OnChange;
    void Initialize();
    void Refresh();
    IEnumerable<TenantDto> GetVisibleTenants(string tenantId);
}
