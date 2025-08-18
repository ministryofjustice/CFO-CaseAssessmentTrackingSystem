using System.Diagnostics;
using System.Runtime.InteropServices;
using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Administration;

[DebuggerDisplay("{Id} {Name} {LocationType}")]
public class Location : BaseAuditableEntity<int>, ILifetime
{

    private string _name;
    private string? _contractId;
    private int _genderProvisionId;
    private Lifetime _lifetime;
    private int? _parentLocationId;
    private readonly List<Location> _childLocations = new();
    private List<LocationMapping> _locationMappings = new();
    private readonly List<Tenant> _tenants = new();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Location()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private Location(string name, int genderProvisionId, LocationType locationType, string contractId,
        DateTime lifetimeStart, DateTime lifetimeEnd)
    {
        _name = name;
        _genderProvisionId = genderProvisionId;
        LocationType = locationType;
        _contractId = contractId;
        _lifetime = new Lifetime(lifetimeStart, lifetimeEnd);
        
        this.AddDomainEvent(new LocationCreatedDomainEvent(this, contractId));
    }

    public static Location Create(string name, int genderProvisionId, LocationType locationType, string contractId,
        DateTime lifetimeStart, DateTime lifetimeEnd)
    {
        return new(name, genderProvisionId, locationType, contractId, lifetimeStart, lifetimeEnd);
    }

    public string Name => _name;

    public GenderProvision GenderProvision => GenderProvision.FromValue(_genderProvisionId);

    public LocationType LocationType { get; private set; }

    public virtual Contract? Contract { get; private set; }

    public Lifetime Lifetime => _lifetime;
    
    public virtual Location? ParentLocation { get; private set; }

    public IReadOnlyCollection<Location> ChildLocations => _childLocations.AsReadOnly();

    public IReadOnlyCollection<LocationMapping> LocationMappings => _locationMappings.AsReadOnly();
    
    public IReadOnlyCollection<Tenant> Tenants => _tenants.AsReadOnly();

    public void AddChildLocation(Location child)
    {
        if (child == null)
        {
            throw new ArgumentNullException(nameof(child));
        }

        child.SetParentLocation(this);
        _childLocations.Add(child);
    }

    public void RemoveChildLocation(Location child)
    {
        if (child == null)
        {
            throw new ArgumentNullException(nameof(child));
        }

        if (ChildLocations.Contains(child))
        {
            child.ClearParentLocation();
            _childLocations.Remove(child);
        }
    }

    private void SetParentLocation(Location parent)
    {
        _parentLocationId = parent.Id;
        ParentLocation = parent;
    }

    private void ClearParentLocation()
    {
        _parentLocationId = null;
        ParentLocation = null;
    }

    public static class Constants
    {
        /// <summary>
        /// Database identifier for an Unmapped Custody Location.
        /// </summary>
        public const int UnmappedCustody = -1;

        /// <summary>
        /// Database identifier for an Unmapped Community Location.
        /// </summary>
        public const int UnmappedCommunity = -2;

        /// <summary>
        /// Database identifier for an Unknown Location.
        /// </summary>
        public const int Unknown = 0;
    }
    
}
