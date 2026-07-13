using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Pipeline;

public abstract class SessionValidatingBehaviour<TRequest, TResponse>(
    ISessionService sessionService,
    ICurrentUserService currentUserService)
{
    protected async Task<TResponse> HandleCore(Func<Task<TResponse>> next)
    {
        var span = SentrySdk.GetSpan()?
            .StartChild("session validation", "Validating session");

        try
        {
            var userId = currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Session is not valid");
            }

            if (await sessionService.IsSessionValidAsync(userId) == false)
            {
                throw new UnauthorizedAccessException("Session is not valid");
            }

            await sessionService.UpdateActivityAsync(userId);
            return await next();
        }
        finally
        {
            span?.Finish();
        }
    }
}

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
        => HandleCore(() => next());
}

public sealed class QuerySessionValidatingBehaviour<TQuery, TResponse>(
    ISessionService sessionService,
    ICurrentUserService currentUserService)
    : SessionValidatingBehaviour<TQuery, TResponse>(sessionService, currentUserService),
        IQueryPipelineBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public Task<TResponse> Handle(
        TQuery query,
        QueryHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
        => HandleCore(() => next());
}
