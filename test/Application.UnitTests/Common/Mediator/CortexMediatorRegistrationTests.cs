using Cfo.Cats.Application;
using Cortex.Mediator.Commands;
using Cortex.Mediator.Notifications;
using Cortex.Mediator.Queries;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Common.Mediator;

public class CortexMediatorRegistrationTests
{
    [Test]
    public void CortexHandlersArePublicAndRegisteredByAddApplication()
    {
        var services = new ServiceCollection();
        services.AddApplication();

        var registeredHandlers = services
            .Where(descriptor => descriptor.ImplementationType is not null)
            .Select(descriptor => (descriptor.ServiceType, descriptor.ImplementationType))
            .ToHashSet();

        var handlerRegistrations = GetExpectedHandlerRegistrations().ToArray();

        var nonPublicHandlers = handlerRegistrations
            .Where(handler => handler.ImplementationType.IsVisible == false)
            .Select(handler => handler.ImplementationType.FullName)
            .Distinct()
            .ToArray();

        nonPublicHandlers.ShouldBeEmpty();

        var missingHandlers = handlerRegistrations
            .Where(handler => registeredHandlers.Contains(handler) == false)
            .Select(handler => $"{handler.ServiceType.FullName} -> {handler.ImplementationType!.FullName}")
            .ToArray();

        missingHandlers.ShouldBeEmpty();
    }

    private static IEnumerable<(Type ServiceType, Type ImplementationType)> GetExpectedHandlerRegistrations() =>
        typeof(DependencyInjection).Assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false, ContainsGenericParameters: false })
            .SelectMany(GetExpectedHandlerRegistrations)
            .Distinct();

    private static IEnumerable<(Type ServiceType, Type ImplementationType)> GetExpectedHandlerRegistrations(
        Type implementationType)
    {
        foreach (var serviceType in implementationType.GetInterfaces().Where(IsCortexHandlerInterface))
        {
            yield return (serviceType, implementationType);
        }
    }

    private static bool IsCortexHandlerInterface(Type type) =>
        type.IsGenericType
        && (type.GetGenericTypeDefinition() == typeof(ICommandHandler<>)
            || type.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
            || type.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)
            || type.GetGenericTypeDefinition() == typeof(INotificationHandler<>));
}
