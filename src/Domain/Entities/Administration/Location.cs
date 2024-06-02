using System.Runtime.InteropServices;
using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Administration;

public class Location : BaseAuditableEntity<int>, ILifetimeEntity
{

    private string _name;
    private string _contractId;
    private int _genderProvisionId;
    private int _locationTypeId;
    private Lifetime _lifetime;
    private int? _parentLocationId;
    private readonly List<Location> _childLocations = new();
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Location()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private Location(string name, int genderProvisionId, int locationTypeId, string contractId,
        DateTime lifetimeStart, DateTime lifetimeEnd)
    {
        _name = name;
        _genderProvisionId = genderProvisionId;
        _locationTypeId = locationTypeId;
        _contractId = contractId;
        _lifetime = new Lifetime(lifetimeStart, lifetimeEnd);
        
        this.AddDomainEvent(new LocationCreatedDomainEvent(this));
    }

    public static Location Create(string name, int genderProvisionId, int locationTypeId, string? contractId,
        DateTime lifetimeStart, DateTime lifetimeEnd)
    {
        return new(name, genderProvisionId, locationTypeId, contractId, lifetimeStart, lifetimeEnd);
    }

    public string Name => _name;

    public GenderProvision GenderProvision => GenderProvision.FromValue(_genderProvisionId);

    public LocationType LocationType => LocationType.FromValue(_locationTypeId);

    public virtual Contract Contract { get; private set; }

    public Lifetime Lifetime => _lifetime;
    
    public virtual Location? ParentLocation { get; private set; }

    public IReadOnlyCollection<Location> ChildLocations => _childLocations.AsReadOnly();


    public void AddChildLocation(Location child)
    {
        if (child == null)
            throw new ArgumentNullException(nameof(child));
        
        child.SetParentLocation(this);
        _childLocations.Add(child);
    }

    public void RemoveChildLocation(Location child)
    {
        if (child == null)
            throw new ArgumentNullException(nameof(child));
        
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
    
}
