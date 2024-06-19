using System.Net.Http.Headers;
using System.Reflection;
using BlazorDownloadFile;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Localization;
using Cfo.Cats.Server.Hubs;
using Cfo.Cats.Server.Middlewares;
using Cfo.Cats.Server.UI.Hubs;
using Cfo.Cats.Server.UI.Services;
using Cfo.Cats.Server.UI.Services.JsInterop;
using Cfo.Cats.Server.UI.Services.Layout;
using Cfo.Cats.Server.UI.Services.Navigation;
using Cfo.Cats.Server.UI.Services.Notifications;
using Cfo.Cats.Server.UI.Services.UserPreferences;
using Hangfire;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using MudBlazor.Services;
using MudExtensions.Services;
using Polly;
using QuestPDF.Infrastructure;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace Cfo.Cats.Server.UI;

public static class DependencyInjection
{
    public static IServiceCollection AddServerUi(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.AddRazorComponents().AddInteractiveServerComponents();
        services.AddCascadingAuthenticationState();
        services.AddScoped<IdentityUserAccessor>();
        services.AddScoped<IdentityRedirectManager>();
        services
            .AddMudBlazorDialog()
            .AddMudServices(mudServicesConfiguration =>
            {
                mudServicesConfiguration.SnackbarConfiguration.PositionClass = Defaults
                    .Classes
                    .Position
                    .BottomRight;
                mudServicesConfiguration.SnackbarConfiguration.PreventDuplicates = false;
                mudServicesConfiguration.SnackbarConfiguration.NewestOnTop = true;
                mudServicesConfiguration.SnackbarConfiguration.ShowCloseIcon = true;
                mudServicesConfiguration.SnackbarConfiguration.VisibleStateDuration = 4000;
                mudServicesConfiguration.SnackbarConfiguration.HideTransitionDuration = 500;
                mudServicesConfiguration.SnackbarConfiguration.ShowTransitionDuration = 500;
                mudServicesConfiguration.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            })
            .AddHotKeys2();

        services.AddFluxor(options =>
        {
            options.ScanAssemblies(Assembly.GetExecutingAssembly());
            //options.UseReduxDevTools();
        });

        services.AddScoped<LocalTimezoneOffset>();
        services.AddHttpContextAccessor();
        services.AddScoped<HubClient>();
        services
            .AddMudExtensions()
            .AddScoped<
                AuthenticationStateProvider,
                IdentityRevalidatingAuthenticationStateProvider
            >()
            .AddScoped<LayoutService>()
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

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });
        return services;
    }

    public static WebApplication ConfigureServer(this WebApplication app, IConfiguration config)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error", true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseStatusCodePagesWithRedirects("/404");
        app.MapHealthChecks("/health");

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self';");
            await next();
        });
        
        app.UseAntiforgery();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"Files")))
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"Files"));
        }

        app.UseStaticFiles(
            new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"Files")
                ),
                RequestPath = new PathString("/Files")
            }
        );

        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(LocalizationConstants.SupportedLanguages.Select(x => x.Code).First())
            .AddSupportedCultures(
                LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray()
            )
            .AddSupportedUICultures(
                LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray()
            );
        app.UseRequestLocalization(localizationOptions);
        app.UseMiddleware<LocalizationCookiesMiddleware>();
        app.UseExceptionHandler();
       
        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
        app.MapHub<ServerHub>(ISignalRHub.Url);

        //QuestPDF License configuration
        QuestPDF.Settings.License = LicenseType.Community;

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();
        app.UseForwardedHeaders();

        return app;
    }
}
