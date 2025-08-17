using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.ClaimTypes;
using ZiggyCreatures.Caching.Fusion;
using Claim = System.Security.Claims.Claim;

namespace Cfo.Cats.Infrastructure.Services;

public class ApplicationUserClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IOptions<IdentityOptions> optionsAccessor,
    IContractService contractService,
    IFusionCache fusionCache)
    : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>(userManager, roleManager, optionsAccessor)
{

    public static string GetCacheKey(string userId)
        => $"ClaimsPrincipal-{userId}";
    
    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        var cacheKey = GetCacheKey(user.Id);

        var principal = await fusionCache.GetOrSetAsync(cacheKey,
            _ => GetPrincipleFromDatabase(user),
            new FusionCacheEntryOptions(TimeSpan.FromHours(8)));

        return principal;
    }

    private async Task<ClaimsPrincipal> GetPrincipleFromDatabase(ApplicationUser user)
    {
        var principal = await base.CreateAsync(user);

        if(principal.Identity is ClaimsIdentity claimsIdentity)
        {
            if (user.DisplayName is not null)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.GivenName, user.DisplayName));
            }

            if (user.TenantId is not null)
            {
                claimsIdentity.AddClaim(new Claim(ApplicationClaimTypes.TenantId, user.TenantId));
                var contracts = contractService.GetVisibleContracts(user.TenantId);

                foreach (var contract in contracts)
                {
                    claimsIdentity.AddClaim(new Claim(ApplicationClaimTypes.Contract, contract.Id));
                }
            }

            if (user.TenantName is not null)
            {
                claimsIdentity.AddClaim(new Claim(ApplicationClaimTypes.TenantName, user.TenantName));
            }

            if (string.IsNullOrEmpty(user.ProfilePictureDataUrl) == false)
            {
                claimsIdentity.AddClaim(new Claim(ApplicationClaimTypes.ProfilePictureDataUrl, user.ProfilePictureDataUrl));
            }

            if (user.UserName!.EndsWith("@justice.gov.uk", StringComparison.CurrentCultureIgnoreCase))
            {
                claimsIdentity.AddClaim(new Claim(ApplicationClaimTypes.InternalStaff, "True"));
            }

            claimsIdentity.AddClaim(user.LockoutEnd is not null
                ? new Claim(ApplicationClaimTypes.AccountLocked, "True")
                : new Claim(ApplicationClaimTypes.AccountLocked, "False"));
        }

        return principal;
    }
}
