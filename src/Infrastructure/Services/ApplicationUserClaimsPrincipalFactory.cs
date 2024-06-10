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
        if (!string.IsNullOrEmpty(user.TenantId))
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(
                new[] { new Claim(ApplicationClaimTypes.TenantId, user.TenantId) }
            );
        }

        if (!string.IsNullOrEmpty(user.TenantName))
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(
                new[] { new Claim(ApplicationClaimTypes.TenantName, user.TenantName) }
            );
        }

        if (!string.IsNullOrEmpty(user.SuperiorId))
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[]
            {
                new Claim(ApplicationClaimTypes.SuperiorId, user.SuperiorId)
            });
        }

        if (!string.IsNullOrEmpty(user.DisplayName))
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(
                new[] { new Claim(ClaimTypes.GivenName, user.DisplayName) }
            );
        }

        if (!string.IsNullOrEmpty(user.ProfilePictureDataUrl))
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(
                new[]
                {
                    new Claim(
                        ApplicationClaimTypes.ProfilePictureDataUrl,
                        user.ProfilePictureDataUrl
                    )
                }
            );
        }

        var appuser = await UserManager.Users.Where(u => u.Id == user.Id).FirstOrDefaultAsync(); // 
        var roles = await UserManager.GetRolesAsync(appuser!);
        if (roles.Count > 0)
        {
            var rolesStr = string.Join(",", roles);
            ((ClaimsIdentity)principal.Identity)?.AddClaims(
                new[] { new Claim(ApplicationClaimTypes.AssignedRoles, rolesStr) }
            );
        }

        return principal;
    }
}
