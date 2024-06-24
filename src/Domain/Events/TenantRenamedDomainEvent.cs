using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Domain.Events;

public class TenantRenamedDomainEvent(Tenant entity, string oldName) : UpdatedDomainEvent<Tenant>(entity)
{
    public string OldName { get; } = oldName;
}
