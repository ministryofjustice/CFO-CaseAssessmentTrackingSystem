using Amazon.Runtime;
using Amazon.S3;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Interfaces.Serialization;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Configurations;
using Cfo.Cats.Infrastructure.Constants.ClaimTypes;
using Cfo.Cats.Infrastructure.Constants.Database;
using Cfo.Cats.Infrastructure.Persistence.Interceptors;
using Cfo.Cats.Infrastructure.Services.Candidates;
using Cfo.Cats.Infrastructure.Services.MultiTenant;
using Cfo.Cats.Infrastructure.Services.Serialization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSettings(configuration)
            .AddDatabase(configuration)
            .AddServices(configuration);

        services.AddAuthenticationService(configuration)
            .AddFusionCacheService();

        services.AddSingleton<IUsersStateContainer, UsersStateContainer>();

        return services;
    }

    private static IServiceCollection AddSettings(
        this IServiceCollection services,
        IConfiguration configuration
    )
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

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();


        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseInMemoryDatabase("CatsDb");
                options.EnableSensitiveDataLogging();
            });
        }
        else
        {
            
            services.AddDbContext<ApplicationDbContext>(
            (p, m) => {
                var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                m.AddInterceptors(p.GetServices<ISaveChangesInterceptor>());
                m.UseDatabase(databaseSettings.DbProvider, databaseSettings.ConnectionString);
            });

        }

        services.AddDbContextFactory<ApplicationDbContext>((serviceProvider, optionsBuilder) =>
        {
            var databaseSettings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            optionsBuilder.AddInterceptors(serviceProvider.GetServices<ISaveChangesInterceptor>());
            optionsBuilder.UseDatabase(databaseSettings.DbProvider, databaseSettings.ConnectionString);
        }, ServiceLifetime.Scoped);
        
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<ApplicationDbContextInitializer>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }

    private static DbContextOptionsBuilder UseDatabase(
        this DbContextOptionsBuilder builder,
        string dbProvider,
        string connectionString
    )
    {
        switch (dbProvider.ToLowerInvariant())
        {
            case DbProviderKeys.SqlServer:
                return builder.UseSqlServer(
                connectionString,
                e => e.MigrationsAssembly("Cfo.Cats.Migrators.MSSQL")
                );

            case DbProviderKeys.SqLite:
                return builder.UseSqlite(
                connectionString,
                e => e.MigrationsAssembly("Cfo.Cats.Migrators.SqLite")
                ).EnableSensitiveDataLogging();

            default:
                throw new InvalidOperationException($"DB Provider {dbProvider} is not supported.");
        }
    }

    private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<PicklistService>()
            .AddSingleton<IPicklistService>(sp => {
                var service = sp.GetRequiredService<PicklistService>();
                service.Initialize();
                return service;
            });

        services
            .AddSingleton<TenantService>()
            .AddSingleton<ITenantService>(sp => {
                var service = sp.GetRequiredService<TenantService>();
                service.Initialize();
                return service;
            });

        services.Configure<NotifyOptions>(configuration.GetSection(NotifyOptions.Notify));

        if(configuration.GetSection("AWS") is {} section && section.Exists())
        {
            var options = configuration.GetAWSOptions();
            options.Credentials = new BasicAWSCredentials(section.GetRequiredValue("AccessKey"), section.GetRequiredValue("SecretKey"));
            services.AddDefaultAWSOptions(options);
            services.AddAWSService<IAmazonS3>();
            
        }
        
        if(configuration["UseDummyCandidateService"] == "True")
        {
            services.AddSingleton<ICandidateService, DummyCandidateService>();
        }
        else
        {
            services.AddHttpClient<ICandidateService, CandidateService>((provider, client) =>
            {
                client.DefaultRequestHeaders.Add("X-API-KEY", configuration.GetRequiredValue("DMS:ApiKey"));
                client.BaseAddress = new Uri(configuration.GetRequiredValue("DMS:ApplicationUrl"));
            });
        }
        
        return services
            .AddSingleton<ISerializer, SystemTextJsonSerializer>()
            .AddScoped<ICurrentUserService, CurrentUserService>()
            .AddScoped<ITenantProvider, TenantProvider>()
            .AddScoped<IValidationService, ValidationService>()
            .AddScoped<IDateTime, DateTimeService>()
            .AddScoped<ICommunicationsService, CommunicationsService>()
            .AddScoped<IExcelService, ExcelService>()
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
            //options.Tokens.EmailConfirmationTokenProvider = "Email";
        });

        services
            .AddScoped<IIdentityService, IdentityService>()
            .AddAuthorizationCore(options => {

                options.AddPolicy(SecurityPolicies.Export, policy => {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
                    policy.RequireRole(RoleNames.SystemSupport, RoleNames.SMT, RoleNames.QAManager);
                    
                });

                options.AddPolicy(SecurityPolicies.CandidateSearch, policy => {
                    policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
                    policy.RequireAuthenticatedUser();
                });

                options.AddPolicy(SecurityPolicies.DocumentUpload, policy => {
                    policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
                    policy.RequireAuthenticatedUser();
                });

                options.AddPolicy(SecurityPolicies.AuthorizedUser, policy => {
                    policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
                    policy.RequireAuthenticatedUser();
                });

                options.AddPolicy(SecurityPolicies.Enrol, policy => {
                    policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
                    policy.RequireAuthenticatedUser();
                });
                
                options.AddPolicy(SecurityPolicies.Import, policy => {
                    policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(RoleNames.SystemSupport, RoleNames.SMT, RoleNames.QAManager);
                });
                
                options.AddPolicy(SecurityPolicies.SystemFunctionsRead, policy => {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
                    policy.RequireRole(RoleNames.SystemSupport, RoleNames.SMT, RoleNames.QAManager, RoleNames.QAOfficer, RoleNames.QASupportManager);
                });
                
                options.AddPolicy(SecurityPolicies.SystemFunctionsWrite, policy => {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
                    policy.RequireRole(RoleNames.SystemSupport, RoleNames.SMT, RoleNames.QAManager, RoleNames.QAOfficer, RoleNames.QASupportManager);
                });
                
                options.AddPolicy(SecurityPolicies.Pqa, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
                    policy.RequireRole(RoleNames.SystemSupport, RoleNames.SMT, RoleNames.QAFinance);
                });
                
                options.AddPolicy(SecurityPolicies.Qa1, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
                    policy.RequireRole(RoleNames.SystemSupport, RoleNames.SMT, RoleNames.QAManager, RoleNames.QASupportManager, RoleNames.QAOfficer);
                });
                
                options.AddPolicy(SecurityPolicies.Qa2, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
                    policy.RequireRole(RoleNames.SystemSupport, RoleNames.SMT, RoleNames.QAManager, RoleNames.QASupportManager);
                });

            })
            .AddAuthentication(options => {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies(options => {});

        services.AddDataProtection().PersistKeysToDbContext<ApplicationDbContext>();

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

        services
            .AddSingleton<UserService>()
            .AddSingleton<IUserService>(sp => {
                var service = sp.GetRequiredService<UserService>();
                service.Initialize();
                return service;
            });
        

        return services;
    }

    public static string GetRequiredValue(this IConfiguration configuration, string name) =>
        configuration[name] ?? throw new InvalidOperationException($"Configuration missing value for: {(configuration is IConfigurationSection s ? s.Path + ":" + name : name)}");
    
    private static IServiceCollection AddFusionCacheService(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services
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
        return services;
    }
}
