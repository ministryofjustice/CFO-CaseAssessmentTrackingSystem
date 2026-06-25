using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Pipeline;

public abstract class AuthorizationBehaviour<TRequest, TResponse>(
    ICurrentUserService currentUserService,
    IIdentityService identityService)
{
    protected async Task<TResponse> HandleCore(
        TRequest request,
        Func<Task<TResponse>> next,
        CancellationToken cancellationToken)
    {
        var span = SentrySdk.GetSpan()?
            .StartChild("authorization", "Authorization checks");

        try
        {
            var authorizeAttributes = request!
                .GetType()
                .GetCustomAttributes<RequestAuthorizeAttribute>()
                .ToArray();

            if (authorizeAttributes.Any() == false)
            {
                var anyUserAttributes = request
                    .GetType()
                    .GetCustomAttributes<AllowAnonymousAttribute>()
                    .SingleOrDefault();

                if (anyUserAttributes == null)
                {
                    throw new UnauthorizedAccessException("Invalid authorization configuration.");
                }
            }

            var userId = currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException();
            }

            var authorizeAttributesWithRoles = authorizeAttributes
                .Where(a => !string.IsNullOrWhiteSpace(a.Roles))
                .ToArray();

            if (authorizeAttributesWithRoles.Any())
            {
                var authorized = false;

                foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                {
                    foreach (var role in roles)
                    {
                        var isInRole = await identityService.IsInRoleAsync(
                            userId,
                            role.Trim(),
                            cancellationToken);

                        if (isInRole)
                        {
                            authorized = true;
                            break;
                        }
                    }
                }

                if (!authorized)
                {
                    throw new ForbiddenException("You are not authorized to access this resource.");
                }
            }

            var authorizeAttributesWithPolicies = authorizeAttributes
                .Where(a => !string.IsNullOrWhiteSpace(a.Policy))
                .ToArray();

            if (authorizeAttributesWithPolicies.Any())
            {
                foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
                {
                    var authorized = await identityService.AuthorizeAsync(
                        userId,
                        policy,
                        cancellationToken);

                    if (!authorized)
                    {
                        throw new ForbiddenException("You are not authorized to access this resource.");
                    }
                }
            }

            return await next();
        }
        finally
        {
            span?.Finish();
        }
    }
}

public sealed class CommandAuthorizationBehaviour<TCommand, TResponse>(
    ICurrentUserService currentUserService,
    IIdentityService identityService)
    : AuthorizationBehaviour<TCommand, TResponse>(currentUserService, identityService),
        ICommandPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public Task<TResponse> Handle(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
        => HandleCore(command, () => next(), cancellationToken);
}

public sealed class QueryAuthorizationBehaviour<TQuery, TResponse>(
    ICurrentUserService currentUserService,
    IIdentityService identityService)
    : AuthorizationBehaviour<TQuery, TResponse>(currentUserService, identityService),
        IQueryPipelineBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public Task<TResponse> Handle(
        TQuery query,
        QueryHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
        => HandleCore(query, () => next(), cancellationToken);
}
