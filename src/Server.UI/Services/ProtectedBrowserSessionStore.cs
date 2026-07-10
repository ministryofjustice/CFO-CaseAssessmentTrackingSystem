using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Cfo.Cats.Server.UI.Services;

public class ProtectedBrowserSessionStore(ProtectedSessionStorage protectedSessionStorage) : ICatsSessionStore
{
    public async Task SetAsync<TItem>(string key, TItem item)
        => await protectedSessionStorage.SetAsync(key, item!);

    public async Task<Result<TItem>> GetAsync<TItem>(string key)
    {
        var result = await protectedSessionStorage.GetAsync<TItem>(key);

        return result is { Success: true, Value: not null }
            ? Result<TItem>.Success(result.Value)
            : Result<TItem>.Failure();
    }
}
