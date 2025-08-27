using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Pipeline;

public class SessionValidatingBehaviour<TRequest, TResponse>(ISessionService sessionService, ICurrentUserService currentUserService) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
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

            if (sessionService.IsSessionValid(userId) == false)
            {
                throw new UnauthorizedAccessException("Session is not valid");
            }

            // session is valid
            sessionService.UpdateActivity(userId);
            return await next(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            span?.Finish();
        }
    }
}
