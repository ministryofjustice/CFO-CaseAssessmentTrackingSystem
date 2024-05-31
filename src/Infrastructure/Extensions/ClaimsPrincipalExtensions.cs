using System.Security.Claims;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Infrastructure.Constants.ClaimTypes;

namespace Cfo.Cats.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static UserProfile GetUserProfileFromClaim(this ClaimsPrincipal claimsPrincipal)
    {
        var profile = new UserProfile()
        {
            Email = "",
            UserId = 0,
            UserName = ""
        };
        if (claimsPrincipal.Identity?.IsAuthenticated ?? false)
        {
            profile.UserId = claimsPrincipal.GetUserId() ?? 0;
            profile.UserName = claimsPrincipal.GetUserName() ?? "";
            profile.TenantId = claimsPrincipal.GetTenantId();
            profile.TenantName = claimsPrincipal.GetTenantName();
            profile.PhoneNumber = claimsPrincipal.GetPhoneNumber();
            profile.SuperiorName = claimsPrincipal.GetSuperiorName();
            profile.SuperiorId = claimsPrincipal.GetSuperiorId();
            profile.Email = claimsPrincipal.GetEmail() ?? "";
            profile.DisplayName = claimsPrincipal.GetDisplayName();
            profile.AssignedRoles = claimsPrincipal.GetRoles();
            profile.DefaultRole = profile.AssignedRoles.Any() ? profile.AssignedRoles.First() : "";
            profile.ProfilePictureDataUrl = claimsPrincipal.GetProfilePictureDataUrl();
            profile.IsActive = true;
        }

        return profile;
    }

    public static string? GetEmail(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.Email);
    }

    public static string? GetPhoneNumber(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.MobilePhone);
    }

    public static int? GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.GetNullableIntValue(ClaimTypes.NameIdentifier);
    }

    public static string? GetUserName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.Name);
    }

    public static string? GetProvider(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.Provider);
    }

    public static string? GetDisplayName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.GivenName);
    }

    public static string? GetProfilePictureDataUrl(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.ProfilePictureDataUrl);
    }

    public static string? GetSuperiorName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.SuperiorName);
    }

    public static int? GetSuperiorId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.GetNullableIntValue(ApplicationClaimTypes.SuperiorId);
    }

    public static string? GetTenantName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.TenantName);
    }

    public static string? GetTenantId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.TenantId);
    }

    public static bool GetStatus(this ClaimsPrincipal claimsPrincipal)
    {
        return Convert.ToBoolean(claimsPrincipal.FindFirstValue(ApplicationClaimTypes.Status));
    }

    public static string? GetAssignRoles(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.AssignedRoles);
    }

    public static string[] GetRoles(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal
            .Claims.Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value)
            .ToArray();
    }

    private static int? GetNullableIntValue(this ClaimsPrincipal claimsPrincipal, string claimType)
    {
        string? value = claimsPrincipal.FindFirstValue(claimType);
        if (int.TryParse( value, out int id ))
        {
            return id;
        }

        return null;
    }
}
