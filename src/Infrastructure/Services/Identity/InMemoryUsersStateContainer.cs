using System.Collections.Concurrent;
using Cfo.Cats.Application.Common.Interfaces.Identity;

namespace Cfo.Cats.Infrastructure.Services.Identity;

/// <summary>
/// In-process presence store used when the SignalR backplane is disabled (single replica / local
/// development). Presence is only correct for connections terminated by this process.
/// </summary>
public sealed class InMemoryUsersStateContainer : IUsersStateContainer
{
    private readonly ConcurrentDictionary<string, string> _usersByConnectionId = new();

    public event Action? OnChange;

    public Task AddOrUpdateAsync(string connectionId, string? name)
    {
        _usersByConnectionId.AddOrUpdate(connectionId, name ?? string.Empty, (_, _) => name ?? string.Empty);
        OnChange?.Invoke();
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string connectionId)
    {
        _usersByConnectionId.TryRemove(connectionId, out _);
        OnChange?.Invoke();
        return Task.CompletedTask;
    }

    public Task<bool> IsOnlineAsync(string name)
    {
        var online = !string.IsNullOrEmpty(name)
            && _usersByConnectionId.Values.Any(v => v.Equals(name, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(online);
    }

    public Task<IReadOnlyCollection<string>> GetOnlineUsersAsync()
    {
        IReadOnlyCollection<string> users = _usersByConnectionId.Values
            .Where(v => !string.IsNullOrEmpty(v))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        return Task.FromResult(users);
    }
}
