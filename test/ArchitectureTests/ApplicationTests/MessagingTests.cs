using System;
using System.Linq;
using System.Reflection;
using Cfo.Cats.Application.Messaging;
using Cfo.Cats.Infrastructure.Messaging.RabbitMq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Domain.ArchitectureTests.ApplicationTests;

public class MessagingTests
{
    private static readonly Assembly ApplicationAssembly = typeof(Application.DependencyInjection).Assembly;

    [Test]
    public void Application_DoesNotReferenceMessageBrokerAssemblies()
    {
        var referenced = ApplicationAssembly.GetReferencedAssemblies()
            .Select(a => a.Name ?? string.Empty)
            .Where(n => n.StartsWith("Rebus", StringComparison.OrdinalIgnoreCase)
                        || n.StartsWith("RabbitMQ", StringComparison.OrdinalIgnoreCase)
                        || n.StartsWith("AWSSDK.SNS", StringComparison.OrdinalIgnoreCase)
                        || n.StartsWith("AWSSDK.SQS", StringComparison.OrdinalIgnoreCase))
            .ToList();

        referenced.ShouldBeEmpty();
    }

    [Test]
    public void EveryIntegrationEvent_ResolvesToAUniqueName()
    {
        // The resolver throws if two events share a name, so constructing it is the assertion.
        var resolver = new EventNameResolver([ApplicationAssembly]);

        var eventTypes = ApplicationAssembly.GetConcreteTypesThatImplement(typeof(IIntegrationEvent));

        eventTypes.ShouldNotBeEmpty();
        foreach (var type in eventTypes)
        {
            var name = resolver.GetName(type);
            resolver.ResolveType(name, resolver.GetVersion(type)).ShouldBe(type);
        }
    }

    [Test]
    public void EveryHandler_HandlesAnIntegrationEvent()
    {
        var handlers = ApplicationAssembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false }
                        && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>)))
            .ToList();

        handlers.ShouldNotBeEmpty();
        var resolver = new EventNameResolver([ApplicationAssembly]);

        foreach (var handler in handlers)
        {
            foreach (var handled in handler.GetInterfaces()
                         .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>))
                         .Select(i => i.GetGenericArguments()[0]))
            {
                Should.NotThrow(() => resolver.GetName(handled), $"{handler.FullName} handles {handled.FullName}, which is not a concrete integration event");
            }
        }
    }
}
