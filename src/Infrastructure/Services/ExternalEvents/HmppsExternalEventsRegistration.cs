using System.Security.Cryptography.X509Certificates;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Features.HmppsExternalApi.ExternalEvents;
using Cfo.Cats.Infrastructure.Configurations;
using Microsoft.Extensions.Configuration;

namespace Cfo.Cats.Infrastructure.Services.ExternalEvents;

/// <summary>
/// Host-agnostic registration for the HMPPS External Events SQS consumer. Both the in-process Blazor
/// host and the out-of-process Worker call <see cref="AddHmppsExternalEvents"/>; no code changes are
/// required to move consumption between them.
/// </summary>
public static class HmppsExternalEventsRegistration
{
    /// <summary>
    /// Name of the <see cref="HttpClient"/> used by <see cref="HmppsAwsCredentialProvider"/> to call
    /// the HMPPS token endpoint.
    /// </summary>
    public const string TokenClientName = "hmpps-external-events-token";

    public static IServiceCollection AddHmppsExternalEvents(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<HmppsExternalApiOptions>(configuration.GetSection(HmppsExternalApiOptions.HmppsExternalApi));

        // Only activate when the HMPPS External API integration is enabled; otherwise leave dormant so
        // environments without the queue / certificate provisioned are unaffected.
        if (configuration.GetValue<bool>("Features:HmppsExternalApi:Enabled") == false)
        {
            return services;
        }

        // HttpClient used to obtain temporary AWS credentials from the HMPPS token endpoint.
        services.AddHttpClient(TokenClientName, (provider, client) =>
            {
                var opts = provider.GetRequiredService<IOptions<HmppsExternalApiOptions>>().Value;
                ConfigureBaseClient(client, opts);
            })
            .ConfigurePrimaryHttpMessageHandler(provider =>
                ConfigureClientCertificate(provider.GetRequiredService<IOptions<HmppsExternalApiOptions>>().Value));

        // Typed HttpClient used to retrieve authoritative event data from the notification URL.
        services.AddHttpClient<IHmppsEventDataRetriever, HmppsEventDataRetriever>((provider, client) =>
            {
                var opts = provider.GetRequiredService<IOptions<HmppsExternalApiOptions>>().Value;
                ConfigureBaseClient(client, opts);
            })
            .ConfigurePrimaryHttpMessageHandler(provider =>
                ConfigureClientCertificate(provider.GetRequiredService<IOptions<HmppsExternalApiOptions>>().Value));

        services.AddSingleton<IAwsCredentialProvider, HmppsAwsCredentialProvider>();
        services.AddSingleton<ISqsClientProvider, SqsClientProvider>();
        services.AddScoped<IHmppsEventDispatcher, HmppsEventDispatcher>();

        // Register all HMPPS event handlers from the Application assembly.
        var handlerTypes = typeof(IHmppsEventHandler).Assembly
            .GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false }
                        && typeof(IHmppsEventHandler).IsAssignableFrom(t));

        foreach (var handler in handlerTypes)
        {
            services.AddScoped(typeof(IHmppsEventHandler), handler);
        }

        services.AddHostedService<HmppsEventConsumer>();

        return services;
    }

    private static void ConfigureBaseClient(HttpClient client, HmppsExternalApiOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            client.BaseAddress = new Uri(options.BaseUrl);
        }

        if (!string.IsNullOrWhiteSpace(options.ApiKey))
        {
            client.DefaultRequestHeaders.Add("X-API-KEY", options.ApiKey);
        }
    }

    private static HttpClientHandler ConfigureClientCertificate(HmppsExternalApiOptions options)
    {
        var handler = new HttpClientHandler();

        if (!string.IsNullOrWhiteSpace(options.ClientCertBase64)
            && !string.IsNullOrWhiteSpace(options.ClientCertPassword))
        {
            var pfxBytes = Convert.FromBase64String(options.ClientCertBase64);
            var certificate = X509CertificateLoader.LoadPkcs12(pfxBytes, options.ClientCertPassword);
            handler.ClientCertificates.Add(certificate);
        }

        return handler;
    }
}
