using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Pipeline;

public sealed class CommandSessionValidatingBehaviour<TCommand, TResponse>(
    ISessionService sessionService,
    ICurrentUserService currentUserService)
    : SessionValidatingBehaviour<TCommand, TResponse>(sessionService, currentUserService),
        ICommandPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public Task<TResponse> Handle(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
        => HandleCore(() => next(), cancellationToken);
}
