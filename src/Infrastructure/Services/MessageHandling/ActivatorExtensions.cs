using System.Reflection;
using Rebus.Activation;
using Rebus.Handlers;

namespace Cfo.Cats.Infrastructure.Services.MessageHandling;

public static class ActivatorExtensions
{
    public static BuiltinHandlerActivator Handle<THandler>(this BuiltinHandlerActivator activator, IServiceProvider provider)
        where THandler: IHandleMessages
    {
        var handlerType = typeof(THandler);

        var interfaceType = handlerType
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleMessages<>));

        if (interfaceType == null)
        {
            throw new InvalidOperationException($"{handlerType.Name} does not implement IHandleMessages<>");
        }

        var messageType = interfaceType.GetGenericArguments()[0];

        var method = typeof(ActivatorExtensions)
            .GetMethod(nameof(HandleInternal), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(messageType, handlerType);

        method.Invoke(null, [activator, provider]);

        return activator;
    }
    
    
    private static BuiltinHandlerActivator HandleInternal<TRequest, THandler>(this BuiltinHandlerActivator activator, IServiceProvider provider)
        where THandler: IHandleMessages<TRequest>
    {
        activator.Handle<TRequest>(async (_, _, message) => {
            using var scope = provider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<THandler>();
            await handler.Handle(message);
        });
        
        return activator;
    }
}
