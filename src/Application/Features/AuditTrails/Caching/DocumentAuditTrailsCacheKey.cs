namespace Cfo.Cats.Application.Features.AuditTrails.Caching;

public static class DocumentAuditTrailsCacheKey
{
    public const string GetAllCacheKey = "all-documentaudittrails";
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromMilliseconds(30);
    private static CancellationTokenSource tokenSource;

    static DocumentAuditTrailsCacheKey()
    {
        tokenSource = new CancellationTokenSource(RefreshInterval);
    }

    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(
            new CancellationChangeToken(SharedExpiryTokenSource().Token)
        );

    public static string GetPaginationCacheKey(string parameters)
    {
        return $"DocumentAuditTrailsWithPaginationQuery,{parameters}";
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
