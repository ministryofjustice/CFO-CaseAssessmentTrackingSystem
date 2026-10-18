using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Pipeline;

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
        => HandleCore(() => next(), cancellationToken);
}
