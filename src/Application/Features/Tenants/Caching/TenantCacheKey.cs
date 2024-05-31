namespace Cfo.Cats.Application.Features.Tenants.Caching;

public static class TenantCacheKey
{
    public const string GetAllCacheKey = "all-Tenants";
    public const string TenantsCacheKey = "all-TenantsCacheKey";
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(1);
    private static CancellationTokenSource tokenSource;

    static TenantCacheKey()
    {
        tokenSource = new CancellationTokenSource(RefreshInterval);
    }

    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(
            new CancellationChangeToken(SharedExpiryTokenSource().Token)
        );

    public static string GetPaginationCacheKey(string parameters)
    {
        return $"TenantsWithPaginationQuery,{parameters}";
    }

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
