using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Initiatives.DTOs;

public static class InitiativeMappings
{
    public static readonly Expression<Func<Initiative, InitiativeDto>> ToDto =
        static entity => new()
        {
            Id = entity.Id,
            Code = entity.Code,
            Description = entity.Description,
            ContractId = entity.Contract!.Id,
            Contract = entity.Contract!.Description,
            TenantId = entity.Contract!.Tenant!.Id,
            LifetimeStart = entity.Lifetime.StartDate,
            LifetimeEnd = entity.Lifetime.EndDate
        };
}
