using Cfo.Cats.Application.Common.Configuration;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Common.Configuration;

public class FeatureTests
{
    private static IConfiguration Build(Dictionary<string, string> values) =>
        new ConfigurationBuilder().AddInMemoryCollection(values!).Build();

    [Test]
    public void GetFeature_BindsScalarBooleanShape()
    {
        var configuration = Build(new() { ["Features:UseWorkerForJobs"] = "true" });

        var feature = configuration.GetFeature(FeatureSections.UseWorkerForJobs);

        feature.Enabled.ShouldBeTrue();
        feature.Tenants.ShouldBeEmpty();
    }

    [Test]
    public void GetFeature_BindsObjectShapeWithTenants()
    {
        var configuration = Build(new()
        {
            ["Features:Redis:Enabled"] = "true",
            ["Features:Redis:Tenants:0"] = "1.2.",
            ["Features:Redis:Tenants:1"] = "3.",
        });

        var feature = configuration.GetFeature(FeatureSections.Redis);

        feature.Enabled.ShouldBeTrue();
        feature.Tenants.ShouldBe(["1.2.", "3."]);
    }

    [Test]
    public void GetFeature_MissingSectionReturnsDisabledFeature()
    {
        var configuration = Build(new());

        var feature = configuration.GetFeature(FeatureSections.HmppsExternalApi);

        feature.Enabled.ShouldBeFalse();
        feature.Tenants.ShouldBeEmpty();
    }

    [Test]
    public void GetFeature_NullConfigurationThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() =>
            FeatureConfigurationExtensions.GetFeature(null!, FeatureSections.Redis));

        exception.ParamName.ShouldBe("configuration");
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void GetFeature_NullOrWhitespaceSectionThrowsArgumentException(string section)
    {
        var configuration = Build(new());

        var exception = Should.Throw<ArgumentException>(() => configuration.GetFeature(section!));

        exception.ParamName.ShouldBe("section");
    }

    [Test]
    public void IsFeatureEnabled_IgnoresTenantAllowList()
    {
        var configuration = Build(new()
        {
            ["Features:HmppsExternalApi:Enabled"] = "true",
            ["Features:HmppsExternalApi:Tenants:0"] = "9.9.",
        });

        configuration.IsFeatureEnabled(FeatureSections.HmppsExternalApi).ShouldBeTrue();
    }

    [Test]
    public void IsFeatureEnabled_NullConfigurationThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() =>
            FeatureConfigurationExtensions.IsFeatureEnabled(null!, FeatureSections.Redis));

        exception.ParamName.ShouldBe("configuration");
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void IsFeatureEnabled_NullOrWhitespaceSectionThrowsArgumentException(string section)
    {
        var configuration = Build(new());

        var exception = Should.Throw<ArgumentException>(() => configuration.IsFeatureEnabled(section!));

        exception.ParamName.ShouldBe("section");
    }

    [Test]
    public void IsEnabledForTenant_DisabledFeatureIsNeverAvailable()
    {
        var feature = new Feature { Enabled = false, Tenants = ["1."] };

        feature.IsEnabledForTenant("1.2.3.").ShouldBeFalse();
    }

    [Test]
    public void IsEnabledForTenant_EmptyAllowListMeansAllTenants()
    {
        var feature = new Feature { Enabled = true };

        feature.IsEnabledForTenant("1.2.3.").ShouldBeTrue();
        feature.IsEnabledForTenant(null).ShouldBeTrue();
    }

    [Test]
    public void IsEnabledForTenant_MatchesTenantExactly()
    {
        var feature = new Feature { Enabled = true, Tenants = ["1.2."] };

        feature.IsEnabledForTenant("1.2.").ShouldBeTrue();
        feature.IsEnabledForTenant("1.2.3.").ShouldBeFalse();
        feature.IsEnabledForTenant("1.3.").ShouldBeFalse();
    }

    [Test]
    public void IsEnabledForTenant_WithAllowListRejectsMissingTenant()
    {
        var feature = new Feature { Enabled = true, Tenants = ["1.2."] };

        feature.IsEnabledForTenant(null).ShouldBeFalse();
        feature.IsEnabledForTenant("").ShouldBeFalse();
    }

    [TestCase("1.", true)]
    [TestCase("1.1.", true)]
    [TestCase("1.1.2.", false)]
    [TestCase("1.2.", false)]
    [TestCase("2.", false)]
    public void ArnsRisk_IsAvailableOnlyToConfiguredTenants(string tenantId, bool expected)
    {
        var configuration = Build(new()
        {
            ["Features:ArnsRisk:Enabled"] = "true",
            ["Features:ArnsRisk:Tenants:0"] = "1.",
            ["Features:ArnsRisk:Tenants:1"] = "1.1.",
        });

        configuration.GetFeature(FeatureSections.ArnsRisk)
            .IsEnabledForTenant(tenantId)
            .ShouldBe(expected);
    }
}
