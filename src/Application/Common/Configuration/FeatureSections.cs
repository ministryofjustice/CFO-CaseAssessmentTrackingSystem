namespace Cfo.Cats.Application.Common.Configuration;

public static class FeatureSections
{
   public const string Root = "Features";

    public const string Redis = $"{Root}:Redis";

    public const string PresenceHub = $"{Root}:PresenceHub";

    public const string HmppsExternalApi = $"{Root}:HmppsExternalApi";

    public const string ArnsRisk = $"{Root}:ArnsRisk";

    public const string IntegrationEventPublishing = $"{Root}:IntegrationEventPublishing";

    public const string UseDmsHardLinkApi = $"{Root}:UseDmsHardLinkApi";

    public const string UseWorkerForJobs = $"{Root}:UseWorkerForJobs";

    public const string UseWorkerForConsumers = $"{Root}:UseWorkerForConsumers";

    public const string EnablePrometheusScrapingEndpoint = $"{Root}:EnablePrometheusScrapingEndpoint";
}
