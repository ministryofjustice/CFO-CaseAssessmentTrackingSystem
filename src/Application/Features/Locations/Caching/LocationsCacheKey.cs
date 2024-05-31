namespace Cfo.Cats.Application.Features.Locations.Caching;

public static class LocationsCacheKey
{
    static LocationsCacheKey()
    {
        tokenSource = new CancellationTokenSource(RefreshInterval);
    }

    private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(1);
    private static CancellationTokenSource tokenSource;

    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(
        new CancellationChangeToken(SharedExpiryTokenSource().Token)
        );

    public static string GetCacheKey(string parameters) => $"GetAllLocationsQuery,{parameters}";

    public static CancellationTokenSource SharedExpiryTokenSource()
    {
        if (tokenSource.IsCancellationRequested)
        {
            tokenSource = new CancellationTokenSource(RefreshInterval);
        }

        return tokenSource;
    }

    public static void Refresh()
    {
        SharedExpiryTokenSource().Cancel();
    }
}
