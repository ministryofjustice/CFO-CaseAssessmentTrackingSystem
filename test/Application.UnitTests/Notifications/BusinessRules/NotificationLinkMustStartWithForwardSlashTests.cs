#nullable enable
using Cfo.Cats.Domain.Entities.Notifications.Rules;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Notifications.BusinessRules;

public class NotificationLinkMustStartWithForwardSlashTests
{
    [Test]
    public void IsBroken_WhenLinkStartsWithForwardSlash_ShouldReturnFalse()
    {
        var rule = new NotificationLinkMustStartWithForwardSlash("/pathway/plan");

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenLinkDoesNotStartWithForwardSlash_ShouldReturnTrue()
    {
        var rule = new NotificationLinkMustStartWithForwardSlash("pathway/plan");

        rule.IsBroken().ShouldBeTrue();
        rule.Message.ShouldBe("Notification links must begin with a forward slash");
    }

    [Test]
    public void IsBroken_WhenLinkIsAbsoluteUrl_ShouldReturnTrue()
    {
        var rule = new NotificationLinkMustStartWithForwardSlash("https://example.com/pathway");

        rule.IsBroken().ShouldBeTrue();
    }

    [Test]
    public void IsBroken_WhenLinkIsEmpty_ShouldReturnTrue()
    {
        var rule = new NotificationLinkMustStartWithForwardSlash("");

        rule.IsBroken().ShouldBeTrue();
    }
}
