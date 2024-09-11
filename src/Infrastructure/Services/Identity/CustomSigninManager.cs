using System.Net;
using Cfo.Cats.Domain.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using NetTools;

namespace Cfo.Cats.Infrastructure.Services.Identity;

public class CustomSigninManager(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<ApplicationUser>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<ApplicationUser> confirmation, IOptions<AllowlistOptions> allowlistOptions, INetworkIpProvider networkIpAccessor)
    : SignInManager<ApplicationUser>(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
{
    
    public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        var user = await UserManager.FindByNameAsync(userName);

        if(user is null)
        {
            return SignInResult.Failed;
        }
        
        var passwordCheckResult = await CheckPasswordSignInAsync(user, password, lockoutOnFailure);

        if(passwordCheckResult.Succeeded is false)
        {
            return passwordCheckResult;
        }

        if (user.IsActive is false)
        {
            return CustomSignInResult.Inactive;
        }

        if (PasswordChecksOutAndRequiresPasswordReset(passwordCheckResult, user))
        {
            return CustomSignInResult.PasswordResetRequired;
        }

        var ipAddress = networkIpAccessor.IpAddress;
        
        if (PasswordCheckSucceededAndTwoFactorDisabledForIpRange(passwordCheckResult, ipAddress))
        {
            await SignInAsync(user, isPersistent);
            return passwordCheckResult;
        }
        
        var signInResult = await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);

        if(signInResult.Succeeded)
        {
            user.LastLogin = DateTime.UtcNow;
            await UserManager.UpdateAsync(user);
        }

        return signInResult;
    }

    private bool PasswordCheckSucceededAndTwoFactorDisabledForIpRange(SignInResult passwordCheckResult, string? ipAddress)
    {
        if (passwordCheckResult.Succeeded == false || string.IsNullOrWhiteSpace(ipAddress))
        {
            return false;
        }
        
        IPAddress? userIp;
        if (IPAddress.TryParse(ipAddress, out userIp) == false)
        {
            return false;
        }

        foreach (var allowedRange in allowlistOptions.Value.AllowedIPs)
        {
            var ipRange = IPAddressRange.Parse(allowedRange);
            if (ipRange.Contains(userIp))
            {
                return true;
            }
        }

        return false;
    }
    private static bool PasswordChecksOutAndRequiresPasswordReset(SignInResult passwordCheckResult, ApplicationUser user) => passwordCheckResult.Succeeded && user.RequiresPasswordReset;

    public class CustomSignInResult : SignInResult
    {
        public bool RequiresPasswordReset { get; private set; }
        public bool IsInactive { get; private set; }

        public static CustomSignInResult PasswordResetRequired => new CustomSignInResult { RequiresPasswordReset = true };
        public static CustomSignInResult Inactive => new CustomSignInResult { IsInactive = true };
    }
    
}