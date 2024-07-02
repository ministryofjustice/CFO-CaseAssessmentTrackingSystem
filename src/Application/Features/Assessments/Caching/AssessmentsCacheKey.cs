namespace Cfo.Cats.Application.Features.Assessments.Caching;

public class AssessmentsCacheKey
{
    //note: we currently only have one but may need more in the future
    public const string GetAllCacheKey = "all-assessments";
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromMinutes(60);
    private static CancellationTokenSource tokenSource;

    static AssessmentsCacheKey()
    {
        tokenSource = new CancellationTokenSource(RefreshInterval);
    }

    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions()
            .AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));

    public static CancellationTokenSource SharedExpiryTokenSource()
    {
        if (tokenSource.IsCancellationRequested) tokenSource = new CancellationTokenSource(RefreshInterval);

        return tokenSource;
    }

    public static void Refresh()
    {
        SharedExpiryTokenSource().Cancel();
    }
}