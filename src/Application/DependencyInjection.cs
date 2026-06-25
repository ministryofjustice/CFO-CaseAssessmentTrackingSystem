
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Mediator;
using Cfo.Cats.Application.Features.Labels;
using Cfo.Cats.Application.Features.ManagementInformation;
using Cfo.Cats.Application.Features.ManagementInformation.Providers;
using Cfo.Cats.Application.Features.ParticipantLabels;
using Cfo.Cats.Application.Features.PerformanceManagement.Providers;
using Cfo.Cats.Application.Pipeline;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.ParticipantLabels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cfo.Cats.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(config => { config.AddMaps(applicationAssembly); });
        services.AddValidatorsFromAssembly(applicationAssembly);
        
        services.AddCortexMediator(new[] { typeof(DependencyInjection) }, config =>
        {
            config.AddOpenCommandPipelineBehavior(typeof(CommandTraceMetricsBehaviour<,>));
            config.AddOpenQueryPipelineBehavior(typeof(QueryTraceMetricsBehaviour<,>));
            config.AddOpenCommandPipelineBehavior(typeof(CommandValidationBehaviour<,>));
            config.AddOpenQueryPipelineBehavior(typeof(QueryValidationBehaviour<,>));
            config.AddOpenCommandPipelineBehavior(typeof(CommandUnhandledExceptionBehaviour<,>));
            config.AddOpenQueryPipelineBehavior(typeof(QueryUnhandledExceptionBehaviour<,>));
            // config.AddOpenQueryPipelineBehavior(typeof(MemoryCacheBehaviour<,>)); // issues with caching means we need to turn it off for now
            config.AddOpenCommandPipelineBehavior(typeof(CommandSessionValidatingBehaviour<,>));
            config.AddOpenQueryPipelineBehavior(typeof(QuerySessionValidatingBehaviour<,>));
            config.AddOpenCommandPipelineBehavior(typeof(CommandAuthorizationBehaviour<,>));
            config.AddOpenQueryPipelineBehavior(typeof(QueryAuthorizationBehaviour<,>));
            // config.AddOpenCommandPipelineBehavior(typeof(CacheInvalidationBehaviour<,>)); // issues with caching means we need to turn it off for now
            config.AddOpenCommandPipelineBehavior(typeof(CommandTransactionBehaviour<,>));
            config.AddOpenQueryPipelineBehavior(typeof(QueryTransactionBehaviour<,>));
            config.AddOpenQueryPipelineBehavior(typeof(AccessAuditingBehaviour<,>));
        });
        services.Replace(ServiceDescriptor.Scoped<IMediator, SequentialNotificationMediator>());

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

        services.AddScoped<ILabelCounter, LabelCounter>();
        services.AddScoped<IParticipantLabelsCounter, ParticipantLabelsCounter>();
        
        return services;
    }
}
