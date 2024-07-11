namespace Cfo.Cats.Application.Features.Candidates.Caching;

public static class CandidatesCacheKey
{
    private static CancellationTokenSource tokenSource;
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromSeconds(60);
    
    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(
            new CancellationChangeToken(SharedExpiryTokenSource().Token)
        );
    
    public static CancellationTokenSource SharedExpiryTokenSource()
    {
        if (tokenSource.IsCancellationRequested)
        {
            tokenSource = new CancellationTokenSource(RefreshInterval);
        }

        return tokenSource;
    }
    
    public static string GetCandidateCacheKey(string parameters)
    {
        return $"EnrolmentsWithPaginationQuery,{parameters}";
    }
    
    public static void Refresh()
    {
        SharedExpiryTokenSource().Cancel();
    }
    
    static CandidatesCacheKey()
    {
        tokenSource = new CancellationTokenSource(RefreshInterval);
    }
}