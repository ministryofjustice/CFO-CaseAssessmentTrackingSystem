using Microsoft.Extensions.Hosting;
using Rebus.Bus;

namespace Cfo.Cats.Infrastructure.Services.MessageHandling;

/// <summary>
/// Forces creation of the singleton one-way <see cref="IBus"/> during host startup, on a
/// normal background thread. Rebus establishes its RabbitMQ connection synchronously when the
/// bus is first created; if that first creation happens lazily inside a Blazor circuit's
/// synchronisation context (e.g. the first time a component publishes), the blocking connect
/// deadlocks the circuit and the page silently fails to load. Warming it here guarantees the
/// bus is already started before any request path resolves it.
/// </summary>
internal sealed class BusWarmupService(IBus bus) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Resolving IBus via the constructor has already created and started the singleton.
        _ = bus;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
