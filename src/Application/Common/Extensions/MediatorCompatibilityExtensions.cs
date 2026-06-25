namespace Cfo.Cats.Application.Common.Extensions;

public static class MediatorCompatibilityExtensions
{
    private static readonly MethodInfo PublishAsyncMethod = typeof(IMediator)
        .GetMethods()
        .Single(method =>
            method.Name == nameof(IMediator.PublishAsync)
            && method.IsGenericMethodDefinition
            && method.GetGenericArguments().Length == 1);

    public static Task<TResult> Send<TResult>(
        this IMediator mediator,
        Cortex.Mediator.Commands.ICommand<TResult> command,
        CancellationToken cancellationToken = default)
        => mediator.SendCommandAsync(command, cancellationToken);

    public static Task Send(
        this IMediator mediator,
        Cortex.Mediator.Commands.ICommand command,
        CancellationToken cancellationToken = default)
        => mediator.SendCommandAsync(command, cancellationToken);

    public static Task<TResult> Send<TResult>(
        this IMediator mediator,
        IQuery<TResult> query,
        CancellationToken cancellationToken = default)
        => mediator.SendQueryAsync(query, cancellationToken);

    public static Task Publish(
        this IMediator mediator,
        INotification notification,
        CancellationToken cancellationToken = default)
    {
        var publishMethod = PublishAsyncMethod.MakeGenericMethod(notification.GetType());
        return (Task)publishMethod.Invoke(mediator, [notification, cancellationToken])!;
    }
}
