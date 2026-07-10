namespace Cfo.Cats.Server.UI.Services;

public interface ICatsSessionStore
{
    /// <summary>
    /// Persists <paramref name="item"/> against <paramref name="key"/> for the current session.
    /// </summary>
    Task SetAsync<TItem>(string key, TItem item);

    /// <summary>
    /// Retrieves the item previously stored against <paramref name="key"/>.
    /// Returns a failed <see cref="Result{TItem}"/> when nothing is stored.
    /// </summary>
    Task<Result<TItem>> GetAsync<TItem>(string key);
}
