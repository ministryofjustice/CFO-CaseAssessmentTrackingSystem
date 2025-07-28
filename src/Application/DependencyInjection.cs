
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Features.ManagementInformation;
using Cfo.Cats.Application.Features.ManagementInformation.Providers;
using Cfo.Cats.Application.Features.PerformanceManagement.Providers;
using Cfo.Cats.Application.Pipeline;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cfo.Cats.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(config => { config.AddMaps(Assembly.GetExecutingAssembly()); });
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
            config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            config.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            config.AddOpenBehavior(typeof(RequestExceptionProcessorBehavior<,>));
            // config.AddOpenBehavior(typeof(MemoryCacheBehaviour<,>)); // issues with caching means we need to turn it off for now
            config.AddOpenBehavior(typeof(SessionValidatingBehaviour<,>));
            config.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
            // config.AddOpenBehavior(typeof(CacheInvalidationBehaviour<,>)); // issues with caching means we need to turn it off for now
            config.AddOpenBehavior(typeof(TransactionBehaviour<,>));
            config.AddOpenBehavior(typeof(AccessAuditingBehaviour<,>));

        });

        // Register a factory for creating new instances of Mediator with a new scope
        services.AddTransient<Func<IMediator>>(sp =>
        {
            return () =>
            {
                var scope = sp.CreateScope();
                return scope.ServiceProvider.GetRequiredService<IMediator>();
            };
        });
        services.AddSingleton<InMemoryTargetsProvider>();
        services.AddSingleton<InMemoryTargetsProviderReprofiled>();
        
        services.AddSingleton<ITargetsProvider>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();

            if (Convert.ToBoolean(configuration["Features:InMemoryTargetsProviderReprofiled"]))
            {
                var inMemory = sp.GetRequiredService<InMemoryTargetsProvider>();
                return new InMemoryTargetsProviderReprofiled(inMemory);
            }

            return sp.GetRequiredService<InMemoryTargetsProvider>();
        });

        services.AddLazyCache();

        services.AddScoped<ICumulativeProvider, CumulativeProvider>();

        services.Scan(scan => scan
            .FromAssemblyOf<IPertinentEventProvider>()
            .AddClasses(classes => classes.AssignableTo<IPertinentEventProvider>())
            .As<IPertinentEventProvider>()
            .WithScopedLifetime());

        return services;
    }
}
