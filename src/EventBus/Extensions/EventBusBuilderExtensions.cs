using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Cfo.Cats.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Cfo.Cats.EventBus.Extensions;

public static class EventBusBuilderExtensions
{
    extension(IEventBusBuilder eventBusBuilder)
    {
        public IEventBusBuilder ConfigureJsonOptions(Action<JsonSerializerOptions> configure)
        {
            eventBusBuilder.Services.Configure<EventBusSubscriptionInfo>(o =>
            {
                configure(o.JsonSerializerOptions);
            });
        
            return eventBusBuilder;
        }

        public IEventBusBuilder AddSubscription<T, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TH>()
            where T : IntegrationEvent
            where TH : class, IIntegrationEventHandler<T>
        {
            // Use keyed services to register multiple handlers for the same event type
            // the consumer can use IKeyedServiceProvider.GetKeyedService<IIntegrationEventHandler>(typeof(T)) to get all
            // handlers for the event type.
            eventBusBuilder.Services.AddKeyedTransient<IIntegrationEventHandler, TH>(typeof(T));

            eventBusBuilder.Services.Configure<EventBusSubscriptionInfo>(o =>
            {
                // Keep track of all registered event types and their name mapping. We send these event types over the message bus
                // and we don't want to do Type.GetType, so we keep track of the name mapping here.

                // This list will also be used to subscribe to events from the underlying message broker implementation.
                o.EventTypes[typeof(T).Name] = typeof(T);
            });

            return eventBusBuilder;
        }
    }
}