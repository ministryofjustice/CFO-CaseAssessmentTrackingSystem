#nullable enable
using Cfo.Cats.Application.Features.Activities.DTOs;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Activities.DTOs;

public class EmploymentDtoValidatorTests
{
    private static EmploymentDto CreateValidDto() => new()
    {
        ParticipantId = "P12345678",
        EmploymentType = "Full time",
        EmployerName = "Acme Ltd",
        Sector = "Education",
        JobTitle = "Teachers",
        JobTitleCode = "2314",
        EmploymentCommenced = DateTime.Today
    };

    [Test]
    public void Validate_WhenSectorIsProvided_ShouldHaveNoSectorError()
    {
        var result = new EmploymentDto.Validator().Validate(CreateValidDto());

        result.Errors.ShouldNotContain(e => e.PropertyName == nameof(EmploymentDto.Sector));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Validate_WhenSectorIsMissingOrEmpty_ShouldHaveSectorError(string? sector)
    {
        var dto = CreateValidDto();
        dto.Sector = sector;

        var result = new EmploymentDto.Validator().Validate(dto);

        result.Errors.ShouldContain(e => e.PropertyName == nameof(EmploymentDto.Sector)
                                         && e.ErrorMessage == "You must choose a Sector");
    }
}
