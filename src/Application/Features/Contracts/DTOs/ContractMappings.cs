using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Contracts.DTOs;

public static class ContractMappings
{
    public static readonly Expression<Func<Contract, ContractDto>> ToDto =
        static entity => new()
        {
            Id = entity.Id,
            Name = entity.Description,
            TenantId = entity.Tenant!.Id
        };
}