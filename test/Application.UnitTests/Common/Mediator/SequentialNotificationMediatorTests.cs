using Cfo.Cats.Application.Common.Mediator;
using Cortex.Mediator;
using Cortex.Mediator.Notifications;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Common.Mediator;

public class SequentialNotificationMediatorTests
{
    [Test]
    public async Task PublishAsyncInvokesNotificationHandlersSequentially()
    {
        var services = new ServiceCollection();
        services.AddScoped<IMediator, SequentialNotificationMediator>();
        services.AddScoped<ConcurrencyProbe>();
        services.AddScoped<INotificationHandler<TestNotification>, FirstNotificationHandler>();
        services.AddScoped<INotificationHandler<TestNotification>, SecondNotificationHandler>();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.PublishAsync(new TestNotification());

        var probe = scope.ServiceProvider.GetRequiredService<ConcurrencyProbe>();
        probe.MaxConcurrentExecutions.ShouldBe(1);
        probe.Events.ShouldBe([
            "first-start",
            "first-end",
            "second-start",
            "second-end"
        ]);
    }

    private sealed class TestNotification : INotification;

    private sealed class FirstNotificationHandler(ConcurrencyProbe probe) : INotificationHandler<TestNotification>
    {
        public Task Handle(TestNotification notification, CancellationToken cancellationToken)
            => probe.Run("first");
    }

    private sealed class SecondNotificationHandler(ConcurrencyProbe probe) : INotificationHandler<TestNotification>
    {
        public Task Handle(TestNotification notification, CancellationToken cancellationToken)
            => probe.Run("second");
    }

    private sealed class ConcurrencyProbe
    {
        private readonly List<string> events = [];
        private int currentExecutions;

        public int MaxConcurrentExecutions { get; private set; }

        public IReadOnlyList<string> Events => events;

        public async Task Run(string handlerName)
        {
            var running = Interlocked.Increment(ref currentExecutions);
            MaxConcurrentExecutions = Math.Max(MaxConcurrentExecutions, running);
            events.Add($"{handlerName}-start");

            await Task.Delay(25);

            events.Add($"{handlerName}-end");
            Interlocked.Decrement(ref currentExecutions);
        }
    }
}
