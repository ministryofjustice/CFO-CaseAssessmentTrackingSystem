using Cfo.Cats.Application.Features.Tenants.DTOs;

namespace Cfo.Cats.Application.Common.Interfaces.MultiTenant;

public interface ITenantService
{
    List<TenantDto> DataSource { get; }
    event Action? OnChange;
    void Initialize();
    void Refresh();
}
