using Cfo.Cats.Application.Common.PublishStrategies;
using Cfo.Cats.Application.Pipeline;
using Cfo.Cats.Application.Pipeline.PreProcessors;
using Microsoft.Extensions.DependencyInjection;

namespace Cfo.Cats.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(config =>
        {
            config.AddMaps(assembly);
        });

        services.AddValidatorsFromAssembly(assembly);

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.NotificationPublisher = new ParallelNoWaitPublisher();
            config.AddRequestPreProcessor(
                typeof(IRequestPreProcessor<>),
                typeof(ValidationPreProcessor<>)
            );
            config.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
            config.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            config.AddOpenBehavior(typeof(RequestExceptionProcessorBehavior<,>));
            config.AddOpenBehavior(typeof(MemoryCacheBehaviour<,>));
            config.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
            config.AddOpenBehavior(typeof(CacheInvalidationBehaviour<,>));
        });

        services.AddLazyCache();

        return services;
    }
}
