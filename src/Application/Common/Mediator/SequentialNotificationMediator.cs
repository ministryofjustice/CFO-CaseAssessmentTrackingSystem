using Microsoft.Extensions.DependencyInjection;

namespace Cfo.Cats.Application.Common.Mediator;

public sealed class SequentialNotificationMediator(IServiceProvider serviceProvider) : IMediator
{
    private readonly IMediator inner = new Cortex.Mediator.Mediator(serviceProvider);

    public Task<TResult> SendCommandAsync<TCommand, TResult>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>
        => inner.SendCommandAsync<TCommand, TResult>(command, cancellationToken);

    public Task<TResult> SendCommandAsync<TResult>(
        ICommand<TResult> command,
        CancellationToken cancellationToken = default)
        => inner.SendCommandAsync(command, cancellationToken);

    public Task SendCommandAsync<TCommand>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand
        => inner.SendCommandAsync(command, cancellationToken);

    public Task SendCommandAsync(
        ICommand command,
        CancellationToken cancellationToken = default)
        => inner.SendCommandAsync(command, cancellationToken);

    public Task<TResult> SendQueryAsync<TQuery, TResult>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>
        => inner.SendQueryAsync<TQuery, TResult>(query, cancellationToken);

    public Task<TResult> SendQueryAsync<TResult>(
        IQuery<TResult> query,
        CancellationToken cancellationToken = default)
        => inner.SendQueryAsync(query, cancellationToken);

    public IAsyncEnumerable<TResult> CreateStream<TQuery, TResult>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : Cortex.Mediator.Streaming.IStreamQuery<TResult>
        => inner.CreateStream<TQuery, TResult>(query, cancellationToken);

    public IAsyncEnumerable<TResult> CreateStream<TResult>(
        Cortex.Mediator.Streaming.IStreamQuery<TResult> query,
        CancellationToken cancellationToken = default)
        => inner.CreateStream(query, cancellationToken);

    public async Task PublishAsync<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        var handlers = serviceProvider.GetServices<INotificationHandler<TNotification>>().ToArray();
        if (handlers.Length == 0)
        {
            return;
        }

        var behaviors = serviceProvider.GetServices<INotificationPipelineBehavior<TNotification>>().ToArray();
        Array.Reverse(behaviors);

        foreach (var handler in handlers)
        {
            NotificationHandlerDelegate handlerDelegate = () => handler.Handle(notification, cancellationToken);

            foreach (var behavior in behaviors)
            {
                var currentDelegate = handlerDelegate;
                handlerDelegate = () => behavior.Handle(notification, currentDelegate, cancellationToken);
            }

            await handlerDelegate();
        }
    }
}
