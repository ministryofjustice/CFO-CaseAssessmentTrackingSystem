using Cfo.Cats.Infrastructure.Constants.Localization;
using Cfo.Cats.Server.Middlewares;
using Cfo.Cats.Server.Services;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cfo.Cats.Server;

public static class DependencyInjection
{
    public static IServiceCollection AddServer(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services
            .AddScoped<LocalizationCookiesMiddleware>()
            .Configure<RequestLocalizationOptions>(options =>
            {
                options.AddSupportedUICultures(
                    LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray()
                );
                options.AddSupportedCultures(
                    LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray()
                );
                options.FallBackToParentUICultures = true;
            })
            .AddLocalization(options =>
                options.ResourcesPath = LocalizationConstants.ResourcesPath
            );

        services.AddMvc();

        services.AddControllers();

        services.AddScoped<IApplicationHubWrapper, ServerHubWrapper>().AddSignalR();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddHealthChecks();

        return services;
    }
}
