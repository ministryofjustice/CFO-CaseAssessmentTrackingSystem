using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Cfo.Cats.Infrastructure.Services.Identity;

public class CustomSigninManager<TUser>(UserManager<TUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<TUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<TUser>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<TUser> confirmation, IHttpContextAccessor httpContextAccessor, IOptions<AllowlistOptions> allowlistOptions)
    : SignInManager<TUser>(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    where TUser : class
{
    public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        var user = await UserManager.FindByNameAsync(userName);

        if (user == null)
        {
            return SignInResult.Failed;
        }

        var ipAddress = httpContextAccessor.HttpContext!.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrWhiteSpace(ipAddress) == false && allowlistOptions.Value.AllowedIPs.Contains(ipAddress))
        {
            var result = await CheckPasswordSignInAsync(user, password, lockoutOnFailure);
            if (result.Succeeded)
            {
                await SignInAsync(user, isPersistent);
            }
            return result;
        }
        return await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
    }

    
}