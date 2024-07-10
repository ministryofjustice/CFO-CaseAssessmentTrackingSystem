namespace Cfo.Cats.Application.Features.Participants.Caching;

public static class ParticipantCacheKey
{
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromSeconds(30);
    private static CancellationTokenSource tokenSource;

    static ParticipantCacheKey()
    {
        tokenSource = new CancellationTokenSource(RefreshInterval);
    }
    
    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(
            new CancellationChangeToken(SharedExpiryTokenSource().Token)
        );
    
    public static string GetCacheKey(string parameters) => $"ParticipantCacheKey,{parameters}";
    public static string GetSummaryCacheKey(string id) => $"ParticipantSummary,{id}";

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