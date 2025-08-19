using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Contracts.Mapping;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
public static class ContractMappings
{
    public static readonly Expression<Func<Contract, ContractDto>> ToDto =
        entity => new()
        {
            Id = entity.Id,
            Name = entity.Description,
            TenantId = entity.Tenant.Id
        };
}
#pragma warning restore CS8602 // Dereference of a possibly null reference.