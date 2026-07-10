using System.Text.Json;
using Cfo.Cats.Application.Common.Interfaces.Serialization;
using StackExchange.Redis;

namespace Cfo.Cats.Server.UI.Services;

/// <summary>
/// <see cref="ICatsSessionStore"/> implementation backed by a shared Redis cache.
/// </summary>
public class RedisSessionStore(IConnectionMultiplexer multiplexer, ICurrentUserService currentUserService) : ICatsSessionStore
{
    /// <summary>
    /// This is currently fixed to 1 hour, once we are on cloud platform this can become a per consumer setting that is passed 
    /// in but for now, as ProtectedSessionStorage has no concept of expiry we are hard coding 1 hour.
    /// </summary>
    private static readonly TimeSpan Expiry = TimeSpan.FromMinutes(60);
    private static readonly JsonSerializerOptions SerializerOptions = CacheJsonSerializerOptions.Options;

    private IDatabase Database => multiplexer.GetDatabase();

    private string NamespacedKey(string key) => $"cats:session:{currentUserService.UserId}:{key}";

    public async Task SetAsync<TItem>(string key, TItem item)
    {
        var json = JsonSerializer.Serialize(item, SerializerOptions);
        await Database.StringSetAsync(NamespacedKey(key), json, Expiry);
    }

    public async Task<Result<TItem>> GetAsync<TItem>(string key)
    {
        var value = await Database.StringGetAsync(NamespacedKey(key));

        if (value.IsNullOrEmpty)
        {
            return Result<TItem>.Failure();
        }

        var item = JsonSerializer.Deserialize<TItem>((string)value!, SerializerOptions);

        return item is not null
            ? Result<TItem>.Success(item)
            : Result<TItem>.Failure();
    }
}
