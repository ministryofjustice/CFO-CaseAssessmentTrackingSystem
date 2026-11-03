using Cfo.Cats.Application.Features.KeyValues.Commands.AddEdit;
using Cfo.Cats.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.KeyValues;

public class AddEditKeyValueCommandValidatorTests
{
    private readonly AddEditKeyValueCommandValidator _validator = new();

    private static AddEditKeyValueCommand Create(string text) => new()
    {
        Name = Picklist.Sector,
        Value = "01",
        Text = text,
        Description = "A Sector option"
    };

    [Test]
    public void Text_WhenExactly200Characters_ShouldBeValid()
    {
        var result = _validator.Validate(Create(new string('a', 200)));

        result.Errors.ShouldNotContain(e => e.PropertyName == nameof(AddEditKeyValueCommand.Text));
    }

    [Test]
    public void Text_WhenOver200Characters_ShouldBeInvalid()
    {
        var result = _validator.Validate(Create(new string('a', 201)));

        result.Errors.ShouldContain(e => e.PropertyName == nameof(AddEditKeyValueCommand.Text)
                                         && e.ErrorMessage == "Text must be between 2 and 200 characters long");
    }

    [TestCase("Water supply; sewerage, waste management and remediation activities")]
    [TestCase("Public administration and defence; compulsory social security")]
    [TestCase("Activities of households as employers and undifferentiated goods - and service-producing activities of households for own use")]
    [TestCase("Telecommunication, computer programming, consulting, computing infrastructure and other information service activities")]
    [TestCase("Incorrect / Missing Paperwork")]
    public void Text_WhenSeededSectorOrPicklistText_ShouldBeValid(string text)
    {
        var result = _validator.Validate(Create(text));

        result.Errors.ShouldNotContain(e => e.PropertyName == nameof(AddEditKeyValueCommand.Text));
    }
}

