using Cfo.Cats.Application.Features.Identity.DTOs;

namespace Cfo.Cats.Application.Common.Interfaces.Identity;

public interface IUserService
{
    IReadOnlyList<ApplicationUserDto> DataSource { get; }

    /// <summary>
    /// Lightweight list of users (no roles/tenant/superior joins), for lookups and filters.
    /// </summary>
    IReadOnlyList<ApplicationUserSummaryDto> Summaries { get; }
    event Action? OnChange;
    void Refresh();
    string? GetDisplayName(string? userId);
}
