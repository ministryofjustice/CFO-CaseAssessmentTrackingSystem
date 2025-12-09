#nullable enable
using Cfo.Cats.Domain.Common;
using Cfo.Cats.Domain.Labels.Rules;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Labels.BusinessRules;

public class GlobalRulesCanOnlyBeDeletedByInternalUsersRuleTests
{
    [Test]
    public void IsBroken_WhenExternalUserDeletesGlobalLabel_ShouldReturnTrue()
    {
        var externalUser = new DomainUser("user-1", "external.user", "1.", false);
        var rule = new GlobalRulesCanOnlyBeDeletedByInternalUsersRule(null, externalUser);

        rule.IsBroken().ShouldBeTrue();
        rule.Message.ShouldBe("You do not have permission to perform this action");
    }

    [Test]
    public void IsBroken_WhenInternalUserDeletesGlobalLabel_ShouldReturnFalse()
    {
        var internalUser = new DomainUser("user-1", "internal.user", "1.", true);
        var rule = new GlobalRulesCanOnlyBeDeletedByInternalUsersRule(null, internalUser);

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenExternalUserDeletesContractLabel_ShouldReturnFalse()
    {
        var externalUser = new DomainUser("user-1", "external.user", "1.", false);
        var rule = new GlobalRulesCanOnlyBeDeletedByInternalUsersRule("CONTRACT-001", externalUser);

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenInternalUserDeletesContractLabel_ShouldReturnFalse()
    {
        var internalUser = new DomainUser("user-1", "internal.user", "1.", true);
        var rule = new GlobalRulesCanOnlyBeDeletedByInternalUsersRule("CONTRACT-001", internalUser);

        rule.IsBroken().ShouldBeFalse();
    }
}
