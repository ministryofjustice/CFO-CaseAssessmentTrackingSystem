using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Pipeline;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IIdentityService identityService;

    public AuthorizationBehaviour(
        ICurrentUserService currentUserService,
        IIdentityService identityService
    )
    {
        this.currentUserService = currentUserService;
        this.identityService = identityService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var authorizeAttributes = request
            .GetType()
            .GetCustomAttributes<RequestAuthorizeAttribute>()
            .ToArray();
        
        if (authorizeAttributes.Any() == false)
        {
            // if we have no authorization attribute, then we must explicitly allow all or error
            var anyUserAttributes = request.GetType()
                .GetCustomAttributes<AllowAnonymousAttribute>()
                .SingleOrDefault();

            if (anyUserAttributes == null)
            {
                throw new UnauthorizedAccessException("Invalid authorization configuration.");
            }
        }

        // Must be authenticated user
        var userId = currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException();
        }

        // DefaultRole-based authorization
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
                        cancellationToken
                    );
                    if (isInRole)
                    {
                        authorized = true;
                        break;
                    }
                }
            }

            // Must be a member of at least one role in roles
            if (!authorized)
            {
                throw new ForbiddenException("You are not authorized to access this resource.");
            }
        }

        // Policy-based authorization
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
                    cancellationToken
                );

                if (!authorized)
                {
                    throw new ForbiddenException(
                        "You are not authorized to access this resource."
                    );
                }
            }
        }

        // User is authorized / authorization not required
        return await next().ConfigureAwait(false);
    }
}
