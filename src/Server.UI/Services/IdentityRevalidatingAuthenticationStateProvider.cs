using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Cfo.Cats.Server.UI.Services;

// This is a server-side AuthenticationStateProvider that revalidates the security stamp for the connected user
// every 10 minutes an interactive circuit is connected.
// if the user has been inactive then they will be locked out
internal sealed class IdentityRevalidatingAuthenticationStateProvider(
    ILoggerFactory loggerFactory,
    IServiceScopeFactory scopeFactory,
    IOptions<IdentityOptions> options
) : RevalidatingServerAuthenticationStateProvider(loggerFactory)
{
    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(10);

    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState,
        CancellationToken cancellationToken
    )
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        return await ValidateSecurityStampAsync(scope.ServiceProvider, authenticationState.User);
    }

    private async Task<bool> ValidateSecurityStampAsync(
        IServiceProvider serviceProvider,
        ClaimsPrincipal principal
    )
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return false;
        }
            
        var principalStamp = principal.FindFirstValue(
            options.Value.ClaimsIdentity.SecurityStampClaimType
        );
        
        // Only fetch the security stamp field
        var context = serviceProvider.GetRequiredService<IApplicationDbContext>();
        var userStamp = await context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.SecurityStamp)
            .FirstOrDefaultAsync();
        
        return principalStamp == userStamp;
    }
}
