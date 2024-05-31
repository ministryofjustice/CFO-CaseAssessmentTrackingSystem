namespace Cfo.Cats.Application.Features.AuditTrails.Caching;

public static class AuditTrailsCacheKey
{
    public const string GetAllCacheKey = "all-audittrails";
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromMilliseconds(30);
    private static CancellationTokenSource tokenSource;

    static AuditTrailsCacheKey()
    {
        tokenSource = new CancellationTokenSource(RefreshInterval);
    }

    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(
            new CancellationChangeToken(SharedExpiryTokenSource().Token)
        );

    public static string GetPaginationCacheKey(string parameters)
    {
        return $"AuditTrailsWithPaginationQuery,{parameters}";
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
