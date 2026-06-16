using Amazon.Runtime;
using Amazon.S3;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Common.Interfaces.Initiatives;
using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Interfaces.Serialization;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;
using Cfo.Cats.Application.Features.Identity.Notifications.IdentityEvents;
using Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;
using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Application.Features.Participants.MessageBus;
using Cfo.Cats.Application.Features.PathwayPlans.IntegrationEvents;
using Cfo.Cats.Application.Features.PRIs.IntegrationEventHandlers;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.ParticipantLabels;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Infrastructure.Configurations;
using Cfo.Cats.Infrastructure.Constants.ClaimTypes;
using Cfo.Cats.Infrastructure.Jobs;
using Cfo.Cats.Infrastructure.Persistence.Converters;
using Cfo.Cats.Infrastructure.Persistence.Interceptors;
using Cfo.Cats.Infrastructure.Persistence.Repositories;
using Cfo.Cats.Infrastructure.Services.Candidates;
using Cfo.Cats.Infrastructure.Services.Contracts;
using Cfo.Cats.Infrastructure.Services.Initiatives;
using Cfo.Cats.Infrastructure.Services.Delius;
using Cfo.Cats.Infrastructure.Services.Jobs;
using Cfo.Cats.Infrastructure.Services.Locations;
using Cfo.Cats.Infrastructure.Services.MessageHandling;
using Cfo.Cats.Infrastructure.Services.MultiTenant;
using Cfo.Cats.Infrastructure.Services.OffLoc;
using Cfo.Cats.Infrastructure.Services.Ordnance;
using Cfo.Cats.Infrastructure.Services.Serialization;
using Dapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.AspNetCore;
using Rebus;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Handlers;
using ZiggyCreatures.Caching.Fusion;
using Cfo.Cats.Application.Features.Identity.MessageBus;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace Cfo.Cats.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddSettings(configuration, environment)
            .AddDatabase(configuration)
            .AddServices(configuration);

        var handlerTypes = typeof(SyncParticipantCommandHandler).Assembly
            .GetTypes()
            .Where(t => t.IsAbstract == false && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleMessages<>)));

        foreach (var handler in handlerTypes)
        {
            services.AddScoped(handler);
        }

        services.AddAuthenticationService(configuration)
            .AddFusionCacheService(configuration);

        // Rebus message consumer background services (CATS only)
        services.AddHostedService<OvernightBackgroundService>();
        services.AddHostedService<TasksBackgroundService>();
        services.AddHostedService<PaymentBackgroundService>();
        services.AddHostedService<DocumentsBackgroundService>();
        services.AddSingleton<ISessionService, SessionService>();

        services.AddSingleton<IUsersStateContainer, UsersStateContainer>();
        services.AddScoped<INetworkIpProvider, NetworkIpProvider>();

        services.AddScoped<INotificationHandler<IdentityAuditNotification>, IdentityCacheClearanceHandler>();

        // Run Quartz in CATS only when a separate Worker process is not handling jobs
        if (!configuration.GetValue<bool>("Features:UseWorkerForJobs"))
        {
            services.AddQuartzJobsAndTriggers(configuration);
            // In-process implementation: talks directly to the local Quartz scheduler
            services.AddScoped<IJobManagementService, QuartzJobManagementService>();
        }
        else
        {
            // Remote implementation: delegates to the Worker's job management REST API.
            // The base URL is resolved via Aspire service discovery ("cats-worker") by default,
            // or overridden with WorkerOptions:BaseUrl for non-Aspire deployments.
            services.AddHttpClient<IJobManagementService, WorkerJobManagementService>(client =>
            {
                var baseUrl = configuration["WorkerOptions:BaseUrl"] ?? "https+http://cats-worker";
                client.BaseAddress = new Uri(baseUrl);
            });
        }

        return services;
    }

    /// <summary>
    /// Registers infrastructure services required by the standalone Worker process.
    /// Use this instead of <see cref="AddInfrastructure"/> when hosting Quartz jobs in a separate Worker.
    /// Does not register Rebus consumer background services or web-specific services.
    /// </summary>
    public static IServiceCollection AddWorkerInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddSettings(configuration, environment)
            .AddDatabase(configuration)
            .AddServices(configuration)
            .AddFusionCacheService(configuration);

        // Localization services are required by FluentValidation validators in the Application layer
        // that inject IStringLocalizer<T>. In the Worker there are no UI resources, so the default
        // no-op localizer (which returns the resource key) is sufficient.
        services.AddLocalization();

        // IHttpContextAccessor is required by CurrentUserService (used by EF interceptors).
        // In a Worker context it returns null for all user properties, which is acceptable for background jobs.
        services.AddHttpContextAccessor();

        // Register a minimal set of Identity services required by the Worker.
        services.AddScoped<IHandleMessages, SyncParticipantCommandHandler>();
        services.AddScoped<IHandleMessages, NotifyInactiveUserCommandHandler>();

        // Quartz always runs in the Worker
        services.AddQuartzJobsAndTriggers(configuration);

        // In-process implementation for the Worker's own job management API endpoints
        services.AddScoped<IJobManagementService, QuartzJobManagementService>();

        return services;
    }

    private static IServiceCollection AddSettings(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services
            .Configure<IdentitySettings>(configuration.GetSection(IdentitySettings.Key))
            .AddSingleton(s => s.GetRequiredService<IOptions<IdentitySettings>>().Value)
            .AddSingleton<IIdentitySettings>(s =>
                s.GetRequiredService<IOptions<IdentitySettings>>().Value
            );
        
        services.Configure<AppConfigurationSettings>(configuration.GetSection(AppConfigurationSettings.Key))
            .AddSingleton(s => s.GetRequiredService<IOptions<AppConfigurationSettings>>().Value)
            .AddSingleton<IApplicationSettings>(s => s.GetRequiredService<IOptions<AppConfigurationSettings>>().Value);

        services
            .Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.Key))
            .AddSingleton(s => s.GetRequiredService<IOptions<DatabaseSettings>>().Value);

        services.Configure<RightToWorkSettings>(configuration.GetSection(RightToWorkSettings.Key))
            .AddSingleton(s => s.GetRequiredService<IOptions<RightToWorkSettings>>().Value)
            .AddSingleton<IRightToWorkSettings>(s =>
                s.GetRequiredService<RightToWorkSettings>()
            );

        services.Configure<RabbitSettings>(configuration.GetSection("RabbitSettings"));

        services.Configure<OvernightServiceSettings>(configuration.GetSection("OvernightServiceSettings"));

        services.AddOptions<OutcomeQualityDipSampleSettings>()
            .BindConfiguration(OutcomeQualityDipSampleSettings.Key)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.Configure<DocumentExportOptions>(options => {
            options.TemplateDirectory = Path.Combine(
                    environment.ContentRootPath,
                    "Files",
                    "Templates"
                );
        });
            

        services.AddSingleton<IBus>(_ =>
        {
            var provider = services.BuildServiceProvider();
            var rabbitSettings = provider.GetRequiredService<IOptions<RabbitSettings>>().Value;

            return Configure.With(new BuiltinHandlerActivator())
                .Transport(t => t.UseRabbitMqAsOneWayClient(configuration.GetConnectionString("rabbit"))
                    .ExchangeNames(rabbitSettings.DirectExchange, rabbitSettings.TopicExchange))
                .Start();
        });

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();        

        services.AddDbContext<ApplicationDbContext>(
            (p, m) => {
                m.AddInterceptors(p.GetServices<ISaveChangesInterceptor>());
                m.UseSqlServer(configuration.GetConnectionString("CatsDb")!,
                    options =>
                    {
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    });
            });
        
        services.AddDbContextFactory<ApplicationDbContext>((serviceProvider, optionsBuilder) =>
        {
            optionsBuilder.AddInterceptors(serviceProvider.GetServices<ISaveChangesInterceptor>());
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("CatsDb")!,
                options =>
                {
                    options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
        }, ServiceLifetime.Scoped);

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        
        services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var connectionString = config.GetConnectionString("CatsDb") ?? throw new InvalidOperationException("Connection string not found");

            return new SqlConnectionFactory(connectionString);
        });

        services.AddScoped<ILabelRepository, LabelRepository>();
        services.AddScoped<IParticipantLabelRepository, ParticipantLabelRepository>();
        services.AddScoped<IInitiativeRepository, InitiativeRepository>();
        
        SqlMapper.AddTypeHandler(typeof(LabelScope), new SmartEnumIntHandler<LabelScope>());
        
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<PicklistService>()
            .AddSingleton<IPicklistService>(sp => {
                var service = sp.GetRequiredService<PicklistService>();
                var cache = sp.GetRequiredService<IFusionCache>();
                var logger = sp.GetRequiredService<ILogger<CachingPicklistService>>();

                return new CachingPicklistService(cache, service, logger);
            })
            .AddSingleton<TenantService>()
            .AddSingleton<ITenantService>(sp =>
            {
                var service = sp.GetRequiredService<TenantService>();
                var cache = sp.GetRequiredService<IFusionCache>();
                var logger = sp.GetRequiredService<ILogger<CachingTenantService>>();

                return new CachingTenantService(cache, service, logger);
            })
            .AddSingleton<LocationService>()
            .AddSingleton<ILocationService>(sp =>
            {
                var service = sp.GetRequiredService<LocationService>();
                var cache = sp.GetRequiredService<IFusionCache>();
                var logger = sp.GetRequiredService<ILogger<CachingLocationService>>();

                return new CachingLocationService(cache, service, logger);
            })
            .AddSingleton<ContractService>()
            .AddSingleton<IContractService>(sp =>
            {
                var service = sp.GetRequiredService<ContractService>();
                var cache = sp.GetRequiredService<IFusionCache>();
                var logger = sp.GetRequiredService<ILogger<CachingContractService>>();

                return new CachingContractService(cache, service, logger);
            })
            .AddSingleton<InitiativeService>()
            .AddSingleton<IInitiativeService>(sp =>
            {
                var service = sp.GetRequiredService<InitiativeService>();
                var cache = sp.GetRequiredService<IFusionCache>();
                var logger = sp.GetRequiredService<ILogger<CachingInitiativeService>>();

                return new CachingInitiativeService(cache, service, logger);
            });
            

        services.Configure<NotifyOptions>(configuration.GetSection(NotifyOptions.Notify));

        var options = configuration.GetAWSOptions();

        string? accessKey = configuration.GetValue<string>("AWS:AccessKey");
        string? secretKey = configuration.GetValue<string>("AWS:SecretKey");

        if (string.IsNullOrEmpty(accessKey) != string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException(
                "AWS configuration is invalid: both AWS:AccessKey and AWS:SecretKey must be set together, or neither should be set.");
        }

        if (string.IsNullOrEmpty(accessKey) is false && string.IsNullOrEmpty(secretKey) is false)
        {
            options.Credentials = new BasicAWSCredentials(accessKey, secretKey);
        }

        services.AddDefaultAWSOptions(options);
        services.AddAWSService<IAmazonS3>();

        services.AddHttpClient<ICandidateService, CandidateService>((provider, client) =>
        {
            client.DefaultRequestHeaders.Add("X-API-KEY", configuration.GetRequiredValue("DMS:ApiKey"));
            client.BaseAddress = new Uri(configuration.GetRequiredValue("DMS:ApplicationUrl"));
        });

        services.AddHttpClient<OfflocService>((_, client) =>
        {
            client.DefaultRequestHeaders.Add("X-API-KEY", configuration.GetRequiredValue("DMS:ApiKey"));
            client.BaseAddress = new Uri(configuration.GetRequiredValue("DMS:ApplicationUrl"));
        });

        services.AddHttpClient<DeliusService>((_, client) =>
        {
            client.DefaultRequestHeaders.Add("X-API-KEY", configuration.GetRequiredValue("DMS:ApiKey"));
            client.BaseAddress = new Uri(configuration.GetRequiredValue("DMS:ApplicationUrl"));
        });
        
        services.AddSingleton<IOfflocService>(sp =>
        {
            var offloc = sp.GetRequiredService<OfflocService>();
            var cache = sp.GetRequiredService<IFusionCache>();

            return new CachingOffLocService(cache, offloc);
        });

        services.AddSingleton<IDeliusService>(sp =>
        {
            var delius = sp.GetRequiredService<DeliusService>();
            var cache = sp.GetRequiredService<IFusionCache>();

            return new CachingDeliusService(cache, delius);
        });
        
        services.AddHttpClient<IAddressLookupService, AddressLookupService>((provider, client) =>
        {
            client.DefaultRequestHeaders.Add("key", configuration.GetRequiredValue("Ordnance:Places:ApiKey"));
            client.BaseAddress = new Uri(configuration.GetRequiredValue("Ordnance:Places:ApplicationUrl"));
        });

        return services
            .AddSingleton<ISerializer, SystemTextJsonSerializer>()
            .AddScoped<ICurrentUserService, CurrentUserService>()
            .AddScoped<ITenantProvider, TenantProvider>()
            .AddScoped<IValidationService, ValidationService>()
            .AddScoped<IDateTime, DateTimeService>()
            .AddScoped<ICommunicationsService, CommunicationsService>()
            .AddScoped<IExcelService, ExcelService>()
            .AddScoped<ICumulativeExcelService, CumulativeExcelService>()
            .AddScoped<IOutcomeQualityDipSampleExcelService, OutcomeQualityDipSampleExcelService>()
            .AddScoped<IUploadService, UploadService>();
    }

    private static IServiceCollection AddAuthenticationService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AllowlistOptions>(configuration.GetSection(nameof(AllowlistOptions)));

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddUserManager<ApplicationUserManager>()
            //.AddSignInManager()
            .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
            .AddDefaultTokenProviders();

        services.AddScoped<UserManager<ApplicationUser>, ApplicationUserManager>();
        services.AddScoped<SignInManager<ApplicationUser>, CustomSigninManager>();
        services.AddScoped<ISecurityStampValidator, SecurityStampValidator<ApplicationUser>>();

        services.Configure<IdentityOptions>(options => {
            var identitySettings = configuration
                .GetRequiredSection(IdentitySettings.Key)
                .Get<IdentitySettings>();

            // Password settings
            options.Password.RequireDigit = identitySettings!.RequireDigit;
            options.Password.RequiredLength = identitySettings.RequiredLength;
            options.Password.RequireNonAlphanumeric = identitySettings.RequireNonAlphanumeric;
            options.Password.RequireUppercase = identitySettings.RequireUpperCase;
            options.Password.RequireLowercase = identitySettings.RequireLowerCase;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(identitySettings.DefaultLockoutTimeSpan * 365);
            options.Lockout.MaxFailedAccessAttempts = identitySettings.MaxFailedAccessAttempts;
            options.Lockout.AllowedForNewUsers = true;

            // Default SignIn settings.
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.SignIn.RequireConfirmedAccount = true;

            // User settings
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = identitySettings.AllowedUserNameCharacters;
            //options.Tokens.EmailConfirmationTokenProvider = "Email";
        });

        services
            .AddScoped<IIdentityService, IdentityService>()
            .AddAuthorizationCore(options => options.AddCatsPolicies())
            .AddAuthentication(options => {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies(options => {});

        services.AddDataProtection()
            .PersistKeysToDbContext<ApplicationDbContext>()
            .SetApplicationName("Cats");

        services.AddSingleton<IPasswordService, PasswordService>();

        CookieSecurePolicy policy = CookieSecurePolicy.SameAsRequest;
        if(configuration["IdentitySettings:SecureCookies"] is not null && configuration["IdentitySettings:SecureCookies"]!.Equals("True", StringComparison.CurrentCultureIgnoreCase))
        {
            policy = CookieSecurePolicy.Always;
        }        

        services.ConfigureApplicationCookie(options => {
            options.LoginPath = "/pages/authentication/login";
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = policy;
        });

        services.AddSingleton<UserService>();
        services.AddSingleton<IUserService>(sp => {
            var service = sp.GetRequiredService<UserService>();
            var cache = sp.GetRequiredService<IFusionCache>();
            var logger =sp.GetRequiredService<ILogger<CachingUserService>>();

            return new CachingUserService(cache, service, logger);
        });

        return services;
    }

    private static IServiceCollection AddQuartzJobsAndTriggers(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetRequiredSection("Quartz");

        services.Configure<QuartzOptions>(options);

        services.AddQuartz(quartz =>
        {
            if (options.GetSection(SyncParticipantsJob.Key.Name).Get<JobOptions>() is 
                { Enabled: true } syncParticipantsJobOptions)
            {
                quartz.AddJob<SyncParticipantsJob>(opts => 
                    opts.WithIdentity(SyncParticipantsJob.Key)
                        .WithDescription(SyncParticipantsJob.Description)
                );

               foreach(var schedule in syncParticipantsJobOptions.CronSchedules)
               {
                   quartz.AddTrigger(opts => opts
                       .ForJob(SyncParticipantsJob.Key)
                       .WithDescription(schedule.Description)
                       .WithCronSchedule(schedule.Chron));
               }

            }
            
            if (options.GetSection(PublishOutboxMessagesJob.Key.Name).Get<JobOptions>() is 
                { Enabled: true } publishOutboxMessagesJob)
            {
                quartz.AddJob<PublishOutboxMessagesJob>(opts => 
                    opts.WithIdentity(PublishOutboxMessagesJob.Key)
                        .WithDescription(PublishOutboxMessagesJob.Description)
                );

                foreach(var schedule in publishOutboxMessagesJob.CronSchedules)
                {
                    quartz.AddTrigger(opts => opts
                        .ForJob(PublishOutboxMessagesJob.Key)
                        .WithDescription(schedule.Description)
                        .WithCronSchedule(schedule.Chron));
                }

            }
            
            if (options.GetSection(DisableDormantAccountsJob.Key.Name).Get<JobOptions>() is 
                { Enabled: true } disableDormantAccountsJob)
            {
                quartz.AddJob<DisableDormantAccountsJob>(opts => 
                    opts.WithIdentity(DisableDormantAccountsJob.Key)
                        .WithDescription(DisableDormantAccountsJob.Description)
                );

                foreach(var schedule in disableDormantAccountsJob.CronSchedules)
                {
                    quartz.AddTrigger(opts => opts
                        .ForJob(DisableDormantAccountsJob.Key)
                        .WithDescription(schedule.Description)
                        .WithCronSchedule(schedule.Chron));
                }

            }
            
            if (options.GetSection(NotifyAccountDeactivationJob.Key.Name).Get<JobOptions>() is 
                { Enabled: true } notifyAccountDeactivationJob)
            {
                quartz.AddJob<NotifyAccountDeactivationJob>(opts => 
                    opts.WithIdentity(NotifyAccountDeactivationJob.Key)
                        .WithDescription(NotifyAccountDeactivationJob.Description)
                );

                foreach(var schedule in notifyAccountDeactivationJob.CronSchedules)
                {
                    quartz.AddTrigger(opts => opts
                        .ForJob(NotifyAccountDeactivationJob.Key)
                        .WithDescription(schedule.Description)
                        .WithCronSchedule(schedule.Chron));
                }

            }

            if (options.GetSection(GenerateOutcomeQualityDipSamplesJob.Key.Name).Get<JobOptions>() is
                { Enabled: true } generateOutcomeQualityDipSamplesJob)
            {
                quartz.AddJob<GenerateOutcomeQualityDipSamplesJob>(opts =>
                    opts.WithIdentity(GenerateOutcomeQualityDipSamplesJob.Key)
                        .WithDescription(GenerateOutcomeQualityDipSamplesJob.Description)
                );

                foreach (var schedule in generateOutcomeQualityDipSamplesJob.CronSchedules)
                {
                    quartz.AddTrigger(opts => opts
                        .ForJob(GenerateOutcomeQualityDipSamplesJob.Key)
                        .WithDescription(schedule.Description)
                        .WithCronSchedule(schedule.Chron));
                }
            }

            if (options.GetSection(ArchiveParticipantsJob.Key.Name).Get<JobOptions>() is
                { Enabled: true } archiveParticipantsJob)
            {
                quartz.AddJob<ArchiveParticipantsJob>(opts =>
                    opts.WithIdentity(ArchiveParticipantsJob.Key)
                        .WithDescription(ArchiveParticipantsJob.Description)
                );

                foreach (var schedule in archiveParticipantsJob.CronSchedules)
                {
                    quartz.AddTrigger(opts => opts
                        .ForJob(ArchiveParticipantsJob.Key)
                        .WithDescription(schedule.Description)
                        .WithCronSchedule(schedule.Chron));
                }
            }
        });

        services.AddQuartzServer(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }

    public static T GetRequiredSection<T>(this IConfiguration configuration, string name) =>
        configuration.GetSection(name).Get<T>() ?? throw new InvalidOperationException($"Configuration missing section for: {(configuration is IConfigurationSection s ? s.Path + ":" + name : name)}");

    public static string GetRequiredValue(this IConfiguration configuration, string name) =>
        configuration[name] ?? throw new InvalidOperationException($"Configuration missing value for: {(configuration is IConfigurationSection s ? s.Path + ":" + name : name)}");
    
    private static IServiceCollection AddFusionCacheService(this IServiceCollection services, IConfiguration configuration)
    {
        var cache = services
            .AddFusionCache()
            .WithDefaultEntryOptions(
                new FusionCacheEntryOptions
                {
                    // CACHE DURATION
                    Duration = TimeSpan.FromMinutes(120),
                    // FAIL-SAFE OPTIONS
                    IsFailSafeEnabled = true,
                    FailSafeMaxDuration = TimeSpan.FromHours(8),
                    FailSafeThrottleDuration = TimeSpan.FromSeconds(30),
                    // FACTORY TIMEOUTS
                    FactorySoftTimeout = TimeSpan.FromMilliseconds(100),
                    FactoryHardTimeout = TimeSpan.FromMilliseconds(1500)
                }
            );

        if(configuration.GetValue<bool>("Features:UseSignalRBackplane"))
        {
            cache.WithSerializer(new FusionCacheSystemTextJsonSerializer(CacheJsonSerializerOptions.Options))
                 .WithDistributedCache(new RedisCache(new RedisCacheOptions { Configuration = configuration.GetConnectionString("redis") }))
                 .WithBackplane(new RedisBackplane(new RedisBackplaneOptions { Configuration = configuration.GetConnectionString("redis") }));
        }

        return services;
    }

    /// <summary>
    /// Registers the minimum ASP.NET Core Identity services needed by the Worker.
    /// Only <see cref="UserManager{TUser}"/> is required (by <see cref="NotifyAccountDeactivationJob"/>).
    /// Does not register SignInManager, authentication cookies, or other web-specific Identity services.
    /// </summary>
    private static IServiceCollection AddWorkerIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<IdentityOptions>(options =>
        {
            var identitySettings = configuration
                .GetRequiredSection(IdentitySettings.Key)
                .Get<IdentitySettings>();

            options.Password.RequireDigit = identitySettings!.RequireDigit;
            options.Password.RequiredLength = identitySettings.RequiredLength;
            options.Password.RequireNonAlphanumeric = identitySettings.RequireNonAlphanumeric;
            options.Password.RequireUppercase = identitySettings.RequireUpperCase;
            options.Password.RequireLowercase = identitySettings.RequireLowerCase;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(identitySettings.DefaultLockoutTimeSpan * 365);
            options.Lockout.MaxFailedAccessAttempts = identitySettings.MaxFailedAccessAttempts;
            options.Lockout.AllowedForNewUsers = true;

            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = identitySettings.AllowedUserNameCharacters;
        });

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddUserManager<ApplicationUserManager>();
            // Note: AddDefaultTokenProviders() is intentionally omitted — it requires
            // IDataProtectionProvider, which is not available in the Worker. The Worker
            // only queries users (NotifyAccountDeactivationJob); it never generates tokens.

        services.AddScoped<UserManager<ApplicationUser>, ApplicationUserManager>();

        return services;
    }
}
