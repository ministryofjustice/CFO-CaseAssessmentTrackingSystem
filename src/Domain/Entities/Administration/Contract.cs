using System.ComponentModel.DataAnnotations;
using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Administration;

public class Contract : BaseAuditableEntity<string>, ILifetimeEntity
{
    private List<Location> _locations = new();
    private readonly string? _tenantId;

    // ef core only
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Contract()
    {
    }
    
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private Contract(string id, int lotNumber, string description, string? tenantId, DateTime startDate, DateTime endDate)
    {
        Id = id;
       
        _tenantId = tenantId;
        
        LotNumber = lotNumber;
        Description = description;
        Lifetime = new Lifetime(startDate, endDate);
        
        AddDomainEvent( new ContractCreatedDomainEvent(this) );
    }
    public int LotNumber { get; private set; }

    public static Contract Create(string id, int lotNumber, string description, string? tenantId, DateTime startDate,
        DateTime endDate) =>
        new(id, lotNumber, description, tenantId, startDate, endDate);

    public IReadOnlyCollection<Location> Locations => _locations.AsReadOnly();

    public Lifetime Lifetime { get; private set; }
    
    public string Description { get; private set; }
    
    public virtual Tenant? Tenant { get; private set; }

}
