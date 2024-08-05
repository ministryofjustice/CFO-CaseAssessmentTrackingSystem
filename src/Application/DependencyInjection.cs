
using Cfo.Cats.Application.Pipeline;
using Cfo.Cats.Application.Pipeline.PreProcessors;
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
            //config.NotificationPublisher = new ParallelNoWaitPublisher();
            //config.AddRequestPreProcessor(typeof(IRequestPreProcessor<>), typeof(ValidationPreProcessor<>));
            config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            config.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
            config.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            config.AddOpenBehavior(typeof(RequestExceptionProcessorBehavior<,>));
            // config.AddOpenBehavior(typeof(MemoryCacheBehaviour<,>)); // issues with caching means we need to turn it off for now
            config.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));

            // config.AddOpenBehavior(typeof(CacheInvalidationBehaviour<,>)); // issues with caching means we need to turn it off for now
            config.AddOpenBehavior(typeof(TransactionBehaviour<,>));
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
        
        services.AddLazyCache();

        return services;
    }
}