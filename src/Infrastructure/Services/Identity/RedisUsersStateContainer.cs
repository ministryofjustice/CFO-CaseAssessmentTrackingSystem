using Cfo.Cats.Application.Common.Interfaces.Identity;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Cfo.Cats.Infrastructure.Services.Identity;

/// <summary>
/// Redis-backed presence store used when the SignalR backplane is enabled (multiple replicas).
///
/// Live connections are held in a single Redis hash keyed by connection id, with the user's
/// display name as the value, so presence is shared across every replica. Changes are announced
/// on a pub/sub channel; each replica subscribes and re-raises <see cref="OnChange"/> locally so
/// that a presence change handled by one replica refreshes pages served by any replica.
/// </summary>
public sealed class RedisUsersStateContainer : IUsersStateContainer, IDisposable
{
    private const string ConnectionsKey = "cats:presence:connections";
    private static readonly RedisChannel ChangeChannel = RedisChannel.Literal("cats:presence:changed");

    private readonly IConnectionMultiplexer _multiplexer;
    private readonly ILogger<RedisUsersStateContainer> _logger;
    private readonly ISubscriber _subscriber;

    public event Action? OnChange;

    public RedisUsersStateContainer(IConnectionMultiplexer multiplexer, ILogger<RedisUsersStateContainer> logger)
    {
        _multiplexer = multiplexer;
        _logger = logger;
        _subscriber = multiplexer.GetSubscriber();
        _subscriber.Subscribe(ChangeChannel, (_, _) => OnChange?.Invoke());
    }

    private IDatabase Database => _multiplexer.GetDatabase();

    public async Task AddOrUpdateAsync(string connectionId, string? name)
    {
        await Database.HashSetAsync(ConnectionsKey, connectionId, name ?? string.Empty).ConfigureAwait(false);
        await NotifyAsync().ConfigureAwait(false);
    }

    public async Task RemoveAsync(string connectionId)
    {
        await Database.HashDeleteAsync(ConnectionsKey, connectionId).ConfigureAwait(false);
        await NotifyAsync().ConfigureAwait(false);
    }

    public async Task<bool> IsOnlineAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        var values = await Database.HashValuesAsync(ConnectionsKey).ConfigureAwait(false);
        return values.Any(v => v.HasValue && string.Equals(v.ToString(), name, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IReadOnlyCollection<string>> GetOnlineUsersAsync()
    {
        var values = await Database.HashValuesAsync(ConnectionsKey).ConfigureAwait(false);
        return values
            .Where(v => v.HasValue)
            .Select(v => v.ToString())
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private async Task NotifyAsync()
    {
        try
        {
            await _subscriber.PublishAsync(ChangeChannel, RedisValue.EmptyString).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // A failure to announce should not break the connect/disconnect flow; presence will
            // simply refresh on the next change rather than immediately.
            _logger.LogWarning(ex, "Failed to publish presence change notification");
        }
    }

    public void Dispose() => _subscriber.Unsubscribe(ChangeChannel);
}
