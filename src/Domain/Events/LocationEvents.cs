using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Domain.Events;

public sealed class LocationCreatedDomainEvent : CreatedDomainEvent<Location>
{
    public LocationCreatedDomainEvent(Location entity, string contractId) 
        : base(entity)
    {
        ContractId = contractId;
    }

    public string ContractId { get; }
    
}