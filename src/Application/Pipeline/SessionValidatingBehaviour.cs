using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Pipeline;

public abstract class SessionValidatingBehaviour<TRequest, TResponse>(
    ISessionService sessionService,
    ICurrentUserService currentUserService)
{
    protected async Task<TResponse> HandleCore(Func<Task<TResponse>> next, CancellationToken cancellationToken)
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

            if (await sessionService.IsSessionValidAsync(userId, cancellationToken) == false)
            {
                throw new UnauthorizedAccessException("Session is not valid");
            }

            await sessionService.UpdateActivityAsync(userId, cancellationToken);
            return await next();
        }
        finally
        {
            span?.Finish();
        }
    }
}
