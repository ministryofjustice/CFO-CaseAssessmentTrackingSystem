using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Administration;

public class Tenant : BaseAuditableEntity<string>
{

    private readonly List<Location> _locations = new();
    private readonly List<TenantDomain> _domains = new();

    public IReadOnlyCollection<Location> Locations => _locations.AsReadOnly();
    public IReadOnlyCollection<TenantDomain> Domains => _domains.AsReadOnly();

#pragma warning disable CS8618// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Tenant()
    {
    }
#pragma warning restore CS8618// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private Tenant(string id, string name, string description, string? contractId)
    {
        Id = id;
        Name = name;
        Description = description;
        ContractId = contractId;

        AddDomainEvent(new TenantCreatedDomainEvent(this));
    }

    public static Tenant Create(string id, string name, string description, string? contractId)
    {
        return new Tenant(id, name, description, contractId);
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public string? ContractId { get; private set; }

    public Tenant Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tenant name cannot be null or empty", nameof(name));
        }

        if (Name != name)
        {
            var oldName = Name;
            Name = name;
            AddDomainEvent(new TenantRenamedDomainEvent(this, oldName));
        }
        return this;
    }

    public Tenant AddLocation(Location location)
    {
        if(_locations.Contains(location) == false)
        {
            _locations.Add(location);
        }
        return this;
    }

    public Tenant RemoveLocation(Location location)
    {
        if(_locations.Contains(location))
        {
            _locations.Remove(location);
        }
        return this;
    }

    public Tenant AddDomain(TenantDomain domain)
    {
        if (_domains.Contains(domain) == false)
        {
            _domains.Add(domain);
        }
        return this;
    }

    public Tenant RemoveDomain(TenantDomain domain)
    {
        if (_domains.Contains(domain))
        {
            _domains.Remove(domain);
        }
        return this;
    }

}
