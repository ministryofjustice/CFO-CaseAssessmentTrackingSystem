using System.Collections.Concurrent;
using Cfo.Cats.Application.Common.Interfaces.Identity;

namespace Cfo.Cats.Infrastructure.Services.Identity;

public class UsersStateContainer : IUsersStateContainer
{
    public ConcurrentDictionary<string, string> UsersByConnectionId { get; } = new();

    public event Action? OnChange;

    public void AddOrUpdate(string connectionId, string? name)
    {
        UsersByConnectionId.AddOrUpdate(
            connectionId,
            name ?? string.Empty,
            (key, oldValue) => name ?? string.Empty
        );
        NotifyStateChanged();
    }

    public void Remove(string connectionId)
    {
        UsersByConnectionId.TryRemove(connectionId, out var _);
        NotifyStateChanged();
    }

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}
