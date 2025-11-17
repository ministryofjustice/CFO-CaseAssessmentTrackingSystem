using System.Text.Json.Serialization;
using Cfo.Cats.EventBus.Extensions;
using Cfo.Cats.EventBusRabbitMQ;
using Cfo.Cats.PaymentProcessor.IntegrationEvents.EventHandling;
using Cfo.Cats.PaymentProcessor.IntegrationEvents.Events;

namespace Cfo.Cats.PaymentProcessor.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.AddRabbitMqEventBus("rabbit")
            .ConfigureJsonOptions(options => options.TypeInfoResolverChain.Add(IntegrationEventContext.Default));

        builder.AddRabbitMqEventBus("rabbit")
            .AddSubscription<PaymentProcessedIntegrationEvent, PaymentProcessedIntegrationEventHandler>();
    }
}

[JsonSerializable(typeof(PaymentProcessedIntegrationEvent))]
public partial class IntegrationEventContext : JsonSerializerContext
{

}