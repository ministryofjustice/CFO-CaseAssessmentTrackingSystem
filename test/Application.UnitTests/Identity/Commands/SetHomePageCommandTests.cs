using Cfo.Cats.Application.Features.Identity.Commands;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Identity.Commands;

public class SetHomePageCommandTests
{
    private static SetHomePage.Validator CreateValidator() => new();

    [Test]
    public void Validator_WithNullHomePage_ShouldPass()
    {
        var validator = CreateValidator();
        var command = new SetHomePage.Command { HomePage = null };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_WithEmptyHomePage_ShouldPass()
    {
        var validator = CreateValidator();
        var command = new SetHomePage.Command { HomePage = string.Empty };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_WithWhitespaceHomePage_ShouldPass()
    {
        var validator = CreateValidator();
        var command = new SetHomePage.Command { HomePage = "   " };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_WithValidPath_ShouldPass()
    {
        var validator = CreateValidator();
        var command = new SetHomePage.Command { HomePage = "/pages/workspace/participant/" };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_WithPathNotStartingWithSlash_ShouldFail()
    {
        var validator = CreateValidator();
        var command = new SetHomePage.Command { HomePage = "pages/workspace/participant/" };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "HomePage");
    }

    [Test]
    public void Validator_WithPathTooLong_ShouldFail()
    {
        var validator = CreateValidator();
        var command = new SetHomePage.Command { HomePage = "/" + new string('a', 50) };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "HomePage");
    }
}
