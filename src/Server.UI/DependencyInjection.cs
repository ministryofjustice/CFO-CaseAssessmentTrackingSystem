using BlazorDownloadFile;
using Cfo.Cats.Infrastructure.Constants.Localization;
using Cfo.Cats.Server.UI.Services;
using Cfo.Cats.Server.UI.Services.Fusion;
using Cfo.Cats.Server.UI.Services.JsInterop;
using Cfo.Cats.Server.UI.Services.Navigation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using MudBlazor.Services;
using MudExtensions.Services;
using ActualLab.Fusion;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using ActualLab.Fusion.Extensions;
using Cfo.Cats.Server.UI.Middlewares;
using Cfo.Cats.Server.UI.Hubs;
using ApexCharts;
using Cfo.Cats.Server.UI.Pages.Workspaces.Provider.Pages.Enrolments;
using Cfo.Cats.Server.UI.Pages.Workspaces.Provider.Pages.Activities;
using StackExchange.Redis;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Infrastructure.Services.Identity;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;

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
                mudServicesConfiguration.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
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
        
        var signalRBuilder = services.AddSignalR(options =>
            {
                options.HandshakeTimeout = TimeSpan.FromSeconds(60);
                options.KeepAliveInterval = TimeSpan.FromSeconds(10);
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(120);
            });

        if (config.GetValue<bool>("Features:PresenceHub:Enabled"))
        {
            services.AddScoped<IHubConnectionFactory, HubConnectionFactory>();
            services.AddScoped<PresenceHubClient>();
        }

        // Configure Redis, SignalR backplane, and session store
        var redisEnabled = config.GetValue<bool>("Features:Redis:Enabled");
        var useRedisSignalRBackplane = redisEnabled && config.GetValue<bool>("Features:Redis:SignalRBackplane");
        var useRedisSessionStore = redisEnabled && config.GetValue<bool>("Features:Redis:SessionStore");

        if (redisEnabled)
        {
            var redisConnectionString = config.GetConnectionString("redis")
                ?? throw new InvalidOperationException("Redis connection must be configured when Features:Redis:Enabled is true. Please set the 'redis' connection string in your configuration.");

            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(redisConnectionString));

            if (useRedisSignalRBackplane)
            {
                signalRBuilder.AddStackExchangeRedis(redisConnectionString, options =>
                    options.Configuration.ChannelPrefix = RedisChannel.Literal("Cats"));
            }
        }

        if (useRedisSignalRBackplane)
        {
            services.AddSingleton<IUsersStateContainer, RedisUsersStateContainer>();
        }
        else
        {
            services.AddSingleton<IUsersStateContainer, InMemoryUsersStateContainer>();
        }

        if (useRedisSessionStore)
        {
            services.AddScoped<ICatsSessionStore, RedisSessionStore>();
        }
        else
        {
            services.AddScoped<ICatsSessionStore, ProtectedBrowserSessionStore>();
        }

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        
        services.AddScoped<LocalTimezoneOffset>();
        services.AddHttpContextAccessor();
/*        services.AddScoped<HubClient>(); */
        services.AddMudExtensions()
            .AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>()
            .AddScoped<DialogServiceHelper>()
            .AddBlazorDownloadFile()
            .AddScoped<IAsyncMenuService, AsyncMenuService>();

        services.AddApexCharts(e =>
        {
            e.GlobalOptions = new ApexChartBaseOptions()
            {
                Theme = new Theme
                {
                    //Palette = new Palette
                    //{
                        
                    //}
                }
            };
        });
        
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        services.AddScoped<CatsSessionStorage>();
        
        return builder;
    }

    public static WebApplication ConfigureServer(this WebApplication app)
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
        
        app.UseAuthentication();
        
        // Add session timeout middleware BEFORE authorization
        app.UseMiddleware<SessionTimeoutMiddleware>();
        
        app.UseAuthorization();

        app.Use((context, next) =>
        {
            context.Request.Scheme = "https";
            return next();
        });

        app.Use(async (context, next) =>
        {
            if (context.Response.Headers.IsReadOnly == false)
            {
                string csp = app.Configuration["Content-Security-Policy"] ??
                                      "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self'; object-src 'self' data:; frame-src 'self' data:;";

                context.Response.Headers.Append("Content-Security-Policy", csp);
            }

            await next();
        });

        app.UseAntiforgery();
        
        //app.UseHttpsRedirection();
        app.UseStaticFiles();

        if (!Directory.Exists(Path.Combine(app.Environment.ContentRootPath, @"Files")))
        {
            Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath, @"Files"));
        }

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, @"Files")),
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
        
        if (app.Configuration.GetValue<bool>("Features:PresenceHub:Enabled"))
        {
            app.MapHub<PresenceHub>(PresenceHub.HubUrl);
        }

        app.MapGet("/.well-known/security.txt", () =>
            Results.Redirect(
                "https://security-guidance.service.justice.gov.uk/.well-known/security.txt",
                permanent: true))
            .AllowAnonymous();

        app.MapAdditionalIdentityEndpoints();
        app.UseForwardedHeaders();
      
        return app;
    }

}
