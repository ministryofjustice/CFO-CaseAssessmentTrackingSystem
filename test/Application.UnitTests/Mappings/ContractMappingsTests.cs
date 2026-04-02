using System.Reflection;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Domain.Entities.Administration;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Mappings;

public class ContractMappingsTests
{
    private static readonly Func<Contract, ContractDto> _toDto = ContractMappings.ToDto.Compile();

    private Contract _contract;
    private Tenant _tenant;

    [SetUp]
    public void SetUp()
    {
        _contract = Contract.Create("C001", 1, "Test Contract Description", "T001", DateTime.Today, DateTime.Today.AddYears(1));
        _tenant = Tenant.Create("T001", "Test Tenant", "Tenant Description", "C001");
        typeof(Contract).GetProperty(nameof(Contract.Tenant))!.SetValue(_contract, _tenant);
    }

    [Test]
    public void ToDto_MapsId()
    {
        var dto = _toDto(_contract);
        dto.Id.ShouldBe("C001");
    }

    [Test]
    public void ToDto_MapsNameFromDescription()
    {
        var dto = _toDto(_contract);
        dto.Name.ShouldBe("Test Contract Description");
    }

    [Test]
    public void ToDto_MapsTenantId()
    {
        var dto = _toDto(_contract);
        dto.TenantId.ShouldBe("T001");
    }
}
