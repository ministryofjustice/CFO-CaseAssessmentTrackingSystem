using System.Reflection;
using Cfo.Cats.Application.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cfo.Cats.Infrastructure.Messaging.RabbitMq;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the RabbitMQ publisher, serializer and event name resolver.
    /// Event types are discovered in <paramref name="eventAssemblies"/>.
    /// </summary>
    public static IServiceCollection AddRabbitMqMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] eventAssemblies)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

        services.TryAddSingleton<IEventNameResolver>(_ => new EventNameResolver(eventAssemblies));
        services.TryAddSingleton<IEventSerializer, JsonEventSerializer>();
        services.TryAddSingleton<RabbitMqConnection>();
        services.TryAddSingleton<RabbitMqIntegrationEventPublisher>();
        services.TryAddSingleton<IIntegrationEventPublisher>(sp => sp.GetRequiredService<RabbitMqIntegrationEventPublisher>());

        services.AddHostedService<RabbitMqWarmupService>();

        return services;
    }

    /// <summary>
    /// Registers a consumer that owns <paramref name="queueName"/> and runs the handlers added in
    /// <paramref name="configure"/>. Requires <see cref="AddRabbitMqMessaging"/>.
    /// </summary>
    public static IServiceCollection AddRabbitMqConsumer(
        this IServiceCollection services,
        string queueName,
        Action<ConsumerBuilder> configure,
        bool transient = false)
    {
        var builder = new ConsumerBuilder();
        configure(builder);

        foreach (var handlerType in builder.HandlerTypes)
        {
            services.TryAddScoped(handlerType);
        }

        var registration = new ConsumerRegistration(queueName, builder.HandlerTypes, transient, Math.Max(1, builder.MaxConcurrency), builder.SingleActiveConsumer);

        services.AddSingleton<IHostedService>(sp => new RabbitMqConsumerHost(
            registration,
            sp.GetRequiredService<RabbitMqConnection>(),
            sp.GetRequiredService<IServiceScopeFactory>(),
            sp.GetRequiredService<IEventSerializer>(),
            sp.GetRequiredService<IEventNameResolver>(),
            sp.GetRequiredService<IOptions<RabbitMqOptions>>(),
            sp.GetRequiredService<ILogger<RabbitMqConsumerHost>>()));

        return services;
    }
}
