using Microsoft.Extensions.Configuration;

namespace Cfo.Cats.Application.Common.Configuration;

public static class FeatureConfigurationExtensions
{
    public static Feature GetFeature(this IConfiguration configuration, string section)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(section);

        var featureSection = configuration.GetSection(section);

        // Scalar form, e.g. "Features:UseWorkerForJobs": true - there are no child keys to bind.
        if (bool.TryParse(featureSection.Value, out var enabled))
        {
            return new Feature { Enabled = enabled };
        }

        return featureSection.Get<Feature>() ?? new Feature();
    }

    public static bool IsFeatureEnabled(this IConfiguration configuration, string section)
        => configuration.GetFeature(section).Enabled;
}
