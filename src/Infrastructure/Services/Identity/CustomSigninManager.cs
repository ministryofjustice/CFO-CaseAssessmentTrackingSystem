using Cfo.Cats.Domain.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Cfo.Cats.Infrastructure.Services.Identity;

public class CustomSigninManager(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<ApplicationUser>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<ApplicationUser> confirmation, IHttpContextAccessor httpContextAccessor, IOptions<AllowlistOptions> allowlistOptions)
    : SignInManager<ApplicationUser>(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
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
                if (user.RequiresPasswordReset)
                {
                    return CustomSignInResult.PasswordResetRequired;
                }

                await SignInAsync(user, isPersistent);
            }

            return result;
        }
        return await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
    }

    public class CustomSignInResult : SignInResult
    {
        public bool RequiresPasswordReset { get; private set; }
        public static CustomSignInResult PasswordResetRequired => new CustomSignInResult { RequiresPasswordReset = true };
    }
    
}