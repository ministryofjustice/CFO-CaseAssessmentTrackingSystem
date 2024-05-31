using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Tenants.DTOs;

[Description("Tenants")]
public record TenantDto
{
    [Description("Tenant Id")]
    public string Id { get; init; }

    [Description("Tenant Name")]
    public string? Name { get; init; }

    [Description("Description")]
    public string? Description { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Tenant, TenantDto>().ReverseMap();
        }
    }
}
