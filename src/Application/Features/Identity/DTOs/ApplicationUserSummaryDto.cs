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

    /// <summary>
    /// Compares users by Id, for selection controls that may receive a different instance of the same user.
    /// </summary>
    public static IEqualityComparer<ApplicationUserSummaryDto?> IdComparer { get; } = new ByIdComparer();

    private sealed class ByIdComparer : IEqualityComparer<ApplicationUserSummaryDto?>
    {
        public bool Equals(ApplicationUserSummaryDto? x, ApplicationUserSummaryDto? y) =>
            ReferenceEquals(x, y) || (x is not null && y is not null && x.Id == y.Id);

        public int GetHashCode(ApplicationUserSummaryDto? obj) => obj?.Id.GetHashCode() ?? 0;
    }

    private class Mapping : Profile
    {
        public Mapping() => CreateMap<ApplicationUser, ApplicationUserSummaryDto>(MemberList.None);
    }
}
