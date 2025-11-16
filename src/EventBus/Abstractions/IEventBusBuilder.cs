using Microsoft.Extensions.DependencyInjection;

namespace Cfo.Cats.EventBus.Abstractions;

public interface IEventBusBuilder
{
    IServiceCollection Services { get; }
}