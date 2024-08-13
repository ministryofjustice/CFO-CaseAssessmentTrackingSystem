using System.Security.Claims;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.ClaimTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Cfo.Cats.Infrastructure.Services;

#nullable disable
public class ApplicationUserClaimsPrincipalFactory
    : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    public ApplicationUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor
    )
        : base(userManager, roleManager, optionsAccessor) { }

    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        var principal = await base.CreateAsync(user);
        ClaimsIdentity claimsIdentity = principal.Identity as ClaimsIdentity;

        if(claimsIdentity is not null)
        {
            claimsIdentity.AddClaims(
                [
                    new Claim(ApplicationClaimTypes.TenantId, user.TenantId!),
                    new Claim(ApplicationClaimTypes.TenantName, user.TenantName),
                    new Claim(ClaimTypes.GivenName, user.DisplayName),
                ]);
            
            if (string.IsNullOrEmpty(user.ProfilePictureDataUrl) == false)
            {
                claimsIdentity.AddClaim(new Claim(ApplicationClaimTypes.ProfilePictureDataUrl, user.ProfilePictureDataUrl));
            }

            if (user.LockoutEnd is not null)
            {
                claimsIdentity.AddClaim(new Claim(ApplicationClaimTypes.AccountLocked, "True"));
            }
            else
            {
                claimsIdentity.AddClaim(new Claim(ApplicationClaimTypes.AccountLocked, "False"));
            }

            var appUser = await UserManager.Users.Where(u => u.Id == user.Id).FirstAsync(); // 
            var roles = await UserManager.GetRolesAsync(appUser!);
            if (roles.Count > 0)
            {
                var rolesStr = string.Join(",", roles);
                claimsIdentity.AddClaim(new Claim(ApplicationClaimTypes.AssignedRoles, rolesStr));
            }
        }

        

        return principal;
    }
}
