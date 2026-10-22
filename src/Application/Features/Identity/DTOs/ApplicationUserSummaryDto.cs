using Cfo.Cats.Domain.Identity;

namespace Cfo.Cats.Application.Features.Identity.DTOs;

/// <summary>
/// Lightweight projection of a user, for embedding in other DTOs.
/// </summary>
[Description("User")]
public class ApplicationUserSummaryDto
{
    [Description("User Id")] public string Id { get; set; } = string.Empty;

    [Description("User Name")] public string UserName { get; set; } = string.Empty;

    [Description("Display Name")] public string DisplayName { get; set; } = string.Empty;

    [Description("Tenant Id")] public string? TenantId { get; set; }

    private class Mapping : Profile
    {
        public Mapping() => CreateMap<ApplicationUser, ApplicationUserSummaryDto>(MemberList.None);
    }
}
