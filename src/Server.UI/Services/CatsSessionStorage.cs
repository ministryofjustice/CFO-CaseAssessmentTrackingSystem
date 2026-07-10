namespace Cfo.Cats.Server.UI.Services;

/// <summary>
/// Per-user UI session storage (for example list filters). A single scoped service is
/// registered for all pages: each consumer chooses its own <c>key</c> and the type of the
/// object it stores, so there is no need for a bespoke class (or DI registration) per page.
///
/// Persistence is delegated to an <see cref="ICatsSessionStore"/> so the backing store
/// (encrypted browser session storage or a shared Redis cache) can be switched via
/// configuration without changing consumers. A stored item is stamped with the id of the
/// user who wrote it and only returned to that same user.
/// </summary>
public class CatsSessionStorage(ICatsSessionStore sessionStore, ICurrentUserService currentUserService)
{
    /// <summary>
    /// Persists <paramref name="item"/> under a key derived from its type name.
    /// </summary>
    public Task SetAsync<TItem>(TItem item) => SetAsync(typeof(TItem).Name, item);

    /// <summary>
    /// Persists <paramref name="item"/> under the supplied <paramref name="key"/>.
    /// </summary>
    public async Task SetAsync<TItem>(string key, TItem item)
    {
        var itemToStore = new CatsSessionItem<TItem>()
        {
            Item = item,
            SessionUserId = currentUserService.UserId!
        };
        await sessionStore.SetAsync(key, itemToStore);
    }

    /// <summary>
    /// Retrieves the item stored under a key derived from <typeparamref name="TItem"/>'s type name.
    /// </summary>
    public Task<Result<TItem>> GetAsync<TItem>() => GetAsync<TItem>(typeof(TItem).Name);

    /// <summary>
    /// Retrieves the item stored under the supplied <paramref name="key"/>.
    /// Returns a failed <see cref="Result{TItem}"/> when nothing is stored for the current user.
    /// </summary>
    public async Task<Result<TItem>> GetAsync<TItem>(string key)
    {
        var result = await sessionStore.GetAsync<CatsSessionItem<TItem>>(key);

        if (result is { Succeeded: true, Data.Item: not null })
        {
            if (result.Data.SessionUserId == currentUserService.UserId)
            {
                return Result<TItem>.Success(result.Data.Item);
            }
        }

        return Result<TItem>.Failure();
    }

    private class CatsSessionItem<TItem>
    {
        /// <summary>
        /// The UserId of the person who is stored this data
        /// </summary>
        public required string SessionUserId { get; set; }

        public required TItem Item { get; set; }
    }

}
