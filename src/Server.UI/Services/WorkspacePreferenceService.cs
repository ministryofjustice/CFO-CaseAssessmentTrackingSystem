using Cfo.Cats.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Server.UI.Services;

/// <summary>
/// Service to notify components when the user's workspace preference (HomePage) changes
/// and to provide a single, shared way of reading the current, up-to-date preference.
/// </summary>
public interface IWorkspacePreferenceService
{
    /// <summary>
    /// Event raised when the user's workspace preference changes.
    /// </summary>
    event Action? OnWorkspacePreferenceChanged;

    /// <summary>
    /// Notifies all listeners that the workspace preference has changed.
    /// </summary>
    void NotifyWorkspacePreferenceChanged();

    /// <summary>
    /// Retrieves the current, untracked HomePage value for the given user directly from the database,
    /// avoiding any stale/cached entity instances.
    /// </summary>
    /// <param name="userId">The ID of the user to look up. Null/empty returns null.</param>
    /// <param name="cancellationToken"></param>
    Task<string?> GetHomePageAsync(string? userId, CancellationToken cancellationToken = default);
}

public class WorkspacePreferenceService(UserManager<ApplicationUser> userManager) : IWorkspacePreferenceService
{
    public event Action? OnWorkspacePreferenceChanged;

    public void NotifyWorkspacePreferenceChanged() => OnWorkspacePreferenceChanged?.Invoke();

    public async Task<string?> GetHomePageAsync(string? userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        var user = await userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user?.HomePage;
    }
}
