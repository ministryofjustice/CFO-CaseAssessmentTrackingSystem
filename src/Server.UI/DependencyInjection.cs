using System.Net.Http.Headers;
using System.Reflection;
using BlazorDownloadFile;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Localization;
using Cfo.Cats.Server.UI.Hubs;
using Cfo.Cats.Server.UI.Services;
using Cfo.Cats.Server.UI.Services.Fusion;
using Cfo.Cats.Server.UI.Services.JsInterop;
using Cfo.Cats.Server.UI.Services.Layout;
using Cfo.Cats.Server.UI.Services.Navigation;
using Cfo.Cats.Server.UI.Services.Notifications;
using Cfo.Cats.Server.UI.Services.UserPreferences;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using MudBlazor.Services;
using MudExtensions.Services;
using Polly;
using ActualLab.Fusion;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using ActualLab.Fusion.Extensions;
using Cfo.Cats.Server.UI.Middlewares;
using Cfo.Cats.Server.UI.Services.Candidate;
using Cfo.Cats.Infrastructure;

namespace Cfo.Cats.Server.UI;

public static class DependencyInjection
{
    public static IServiceCollection AddServerUi(this IServiceCollection services, IConfiguration config)
    {
        services.AddRazorComponents().AddInteractiveServerComponents();
        services.AddCascadingAuthenticationState();
        services.AddScoped<IdentityUserAccessor>();
        services.AddScoped<IdentityRedirectManager>();
        services
            .AddMudBlazorDialog()
            .AddMudServices(mudServicesConfiguration => {
                mudServicesConfiguration.SnackbarConfiguration.PositionClass = Defaults
                    .Classes
                    .Position
                    .TopCenter;
                mudServicesConfiguration.SnackbarConfiguration.PreventDuplicates = false;
                mudServicesConfiguration.SnackbarConfiguration.NewestOnTop = true;
                mudServicesConfiguration.SnackbarConfiguration.ShowCloseIcon = true;
                mudServicesConfiguration.SnackbarConfiguration.VisibleStateDuration = 3000;
                mudServicesConfiguration.SnackbarConfiguration.HideTransitionDuration = 500;
                mudServicesConfiguration.SnackbarConfiguration.ShowTransitionDuration = 500;
                mudServicesConfiguration.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
                mudServicesConfiguration.SnackbarConfiguration.PreventDuplicates = false;
            });

        services.AddMudPopoverService(options => {
            options.ThrowOnDuplicateProvider = false;
        });
        
        services.AddMudBlazorSnackbar();
        services.AddMudBlazorDialog();
        services.AddHotKeys2();
        
        services.AddFluxor(options =>
        {
            options.ScanAssemblies(Assembly.GetExecutingAssembly());
        });

        services.AddFusion(fusion => {
            fusion.AddInMemoryKeyValueStore();
            fusion.AddService<IUserSessionTracker, UserSessionTracker>();
            fusion.AddService<IOnlineUserTracker, OnlineUserTracker>();
        });

        services.AddScoped<LocalizationCookiesMiddleware>()
            .Configure<RequestLocalizationOptions>(options =>
            {
                options.AddSupportedUICultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
                options.AddSupportedCultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
                options.FallBackToParentUICultures = true;
            })
            .AddLocalization(options => options.ResourcesPath = LocalizationConstants.ResourcesPath);

        services.AddMvc();
        services.AddControllers();
        
        services.AddScoped<IApplicationHubWrapper, ServerHubWrapper>()
            .AddSignalR();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddHealthChecks();
        
        services.AddScoped<LocalTimezoneOffset>();
        services.AddHttpContextAccessor();
        services.AddScoped<HubClient>();
        services.AddMudExtensions()
            .AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>()
            .AddScoped<LayoutService>()
            .AddScoped<DialogServiceHelper>()
            .AddBlazorDownloadFile()
            .AddScoped<IUserPreferencesService, UserPreferencesService>()
            .AddScoped<IMenuService, MenuService>()
            .AddScoped<InMemoryNotificationService>()
            .AddScoped<INotificationService>(sp =>
            {
                var service = sp.GetRequiredService<InMemoryNotificationService>();
                service.Preload();
                return service;
            });

        services.AddHttpClient<CandidateService>((provider, client) =>
        {
            client.DefaultRequestHeaders.Add("X-API-KEY", config.GetRequiredValue("DMS:ApiKey"));
            client.BaseAddress = new Uri(config.GetRequiredValue("DMS:ApplicationUrl"));
        });

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });
        
        return services;
    }

    public static WebApplication ConfigureServer(this WebApplication app, IConfiguration configuration)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error", true);
            app.UseHsts();
        }

        app.UseStatusCodePagesWithRedirects("/404");
        app.MapHealthChecks("/health");

        app.UseAuthentication();
        app.UseAuthorization();

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self'; object-src 'self' data:; frame-src 'self' data:;"); 
            await next();
        });
        
        app.UseAntiforgery();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"Files")))
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"Files"));
        }

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
            RequestPath = new PathString("/Files")
        });

        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(LocalizationConstants.SupportedLanguages.Select(x => x.Code).First())
            .AddSupportedCultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray())
            .AddSupportedUICultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
        app.UseRequestLocalization(localizationOptions);
        app.UseMiddleware<LocalizationCookiesMiddleware>();
        app.UseExceptionHandler();
        
        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
        app.MapHub<ServerHub>(ISignalRHub.Url);
        
        app.MapAdditionalIdentityEndpoints();
        app.UseForwardedHeaders();
        app.UseWebSockets(new WebSocketOptions()
        { 
            // We obviously need this
            KeepAliveInterval = TimeSpan.FromSeconds(30), // Just in case
        });
        
        return app;
    }

}
