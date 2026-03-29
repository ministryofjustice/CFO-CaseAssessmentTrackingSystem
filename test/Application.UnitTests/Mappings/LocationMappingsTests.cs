using System.Reflection;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Mappings;

public class LocationMappingsTests
{
    private static readonly Func<Location, LocationDto> _toDto = LocationMappings.ToDto.Compile();

    private Location _location;
    private Location _parentLocation;
    private Contract _contract;
    private Tenant _tenant;

    [SetUp]
    public void SetUp()
    {
        _contract = Contract.Create("C001", 1, "Test Contract", "T001", DateTime.Today, DateTime.Today.AddYears(1));
        _tenant = Tenant.Create("T001", "Test Tenant", "Tenant Description", "C001");
        _parentLocation = Location.Create("Parent Location", GenderProvision.Any.Value, LocationType.Feeder, "C001", DateTime.Today, DateTime.Today.AddYears(1));
        _location = Location.Create("Test Wing", GenderProvision.Male.Value, LocationType.Wing, "C001", DateTime.Today, DateTime.Today.AddYears(1));

        typeof(Location).GetProperty(nameof(Location.Contract))!.SetValue(_location, _contract);
        typeof(Location).GetProperty(nameof(Location.ParentLocation))!.SetValue(_location, _parentLocation);

        var tenantsList = (List<Tenant>)typeof(Location)
            .GetField("_tenants", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(_location)!;
        tenantsList.Add(_tenant);
    }

    [Test]
    public void ToDto_MapsName()
    {
        var dto = _toDto(_location);
        dto.Name.ShouldBe("Test Wing");
    }

    [Test]
    public void ToDto_MapsGenderProvision()
    {
        var dto = _toDto(_location);
        dto.GenderProvision.ShouldBe(GenderProvision.Male);
    }

    [Test]
    public void ToDto_MapsLocationType()
    {
        var dto = _toDto(_location);
        dto.LocationType.ShouldBe(LocationType.Wing);
    }

    [Test]
    public void ToDto_MapsContractNameFromDescription()
    {
        var dto = _toDto(_location);
        dto.ContractName.ShouldBe("Test Contract");
    }

    [Test]
    public void ToDto_MapsParentLocationId()
    {
        var dto = _toDto(_location);
        dto.ParentLocationId.ShouldBe(_parentLocation.Id);
    }

    [Test]
    public void ToDto_MapsParentLocationName()
    {
        var dto = _toDto(_location);
        dto.ParentLocationName.ShouldBe("Parent Location");
    }

    [Test]
    public void ToDto_MapsTenants()
    {
        var dto = _toDto(_location);
        dto.Tenants.ShouldBe(new[] { "T001" });
    }
}
