namespace Cfo.Cats.Application.Common.Interfaces.Identity;

/// <summary>
/// Tracks which users currently have a live connection to the site so the UI can show presence
/// ("who is online"). The backing store must be shared across replicas when the SignalR backplane
/// is enabled, otherwise presence would only reflect connections terminated by the local replica.
/// </summary>
public interface IUsersStateContainer
{
    /// <summary>
    /// Raised when presence changes. With a shared (Redis) store this fires on every replica so
    /// that pages on any replica can refresh, not just the one that handled the connection change.
    /// </summary>
    event Action? OnChange;

    /// <summary>Records (or refreshes) the display name associated with a live connection.</summary>
    Task AddOrUpdateAsync(string connectionId, string? name);

    /// <summary>Removes a connection, e.g. when it disconnects.</summary>
    Task RemoveAsync(string connectionId);

    /// <summary>Returns true if the given display name has at least one live connection.</summary>
    Task<bool> IsOnlineAsync(string name);

    /// <summary>Returns the distinct display names that currently have a live connection.</summary>
    Task<IReadOnlyCollection<string>> GetOnlineUsersAsync();
}
