using System.Net.Http.Headers;
using System.Reflection;
using BlazorDownloadFile;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Localization;
using Cfo.Cats.Server.UI.Services;
using Cfo.Cats.Server.UI.Services.Fusion;
using Cfo.Cats.Server.UI.Services.JsInterop;
using Cfo.Cats.Server.UI.Services.Layout;
using Cfo.Cats.Server.UI.Services.Navigation;
using Cfo.Cats.Server.UI.Services.UserPreferences;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using MudBlazor.Services;
using MudExtensions.Services;
using ActualLab.Fusion;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using ActualLab.Fusion.Extensions;
using Cfo.Cats.Server.UI.Middlewares;
using Cfo.Cats.Infrastructure;
using Cfo.Cats.Infrastructure.Services;

namespace Cfo.Cats.Server.UI;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddServerUi(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var config = builder.Configuration;
        var environment = builder.Environment;
        
        
        CookieSecurePolicy policy = CookieSecurePolicy.SameAsRequest;
        if(config["IdentitySettings:SecureCookies"] is not null && config["IdentitySettings:SecureCookies"]!.Equals("True", StringComparison.CurrentCultureIgnoreCase))
        {
            policy = CookieSecurePolicy.Always;
        }
    
        services.AddAntiforgery(options =>
        {
            options.Cookie.SecurePolicy = policy;
        });

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
        
        services.AddSignalR(options =>
            {
                options.HandshakeTimeout = TimeSpan.FromSeconds(60); // Adjust as needed
                options.KeepAliveInterval = TimeSpan.FromSeconds(10); // SignalR keep-alive interval
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(120); // SignalR client timeout interval
            });
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddHealthChecks();
        
        services.AddScoped<LocalTimezoneOffset>();
        services.AddHttpContextAccessor();
/*        services.AddScoped<HubClient>(); */
        services.AddMudExtensions()
            .AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>()
            .AddScoped<LayoutService>()
            .AddScoped<DialogServiceHelper>()
            .AddBlazorDownloadFile()
            .AddScoped<IUserPreferencesService, UserPreferencesService>()
            .AddScoped<IMenuService, MenuService>();

        
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });
        
        return builder;
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


        app.Use((context, next) =>
        {
            context.Request.Scheme = "https";
            return next();
        });

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self'; object-src 'self' data:; frame-src 'self' data:;"); 
            await next();
        });
        
        app.UseAntiforgery();
        
        //app.UseHttpsRedirection();
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
        
        app.MapAdditionalIdentityEndpoints();
        app.UseForwardedHeaders();
      
        return app;
    }

}
