﻿using DocumentFormat.OpenXml.Office2019.Excel.RichData;

namespace Cfo.Cats.Application.Features.Documents.Caching;

public static class DocumentCacheKey
{
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromSeconds(30);
    private static CancellationTokenSource tokenSource;

    static DocumentCacheKey() 
        => tokenSource = new CancellationTokenSource(RefreshInterval);

    public static CancellationTokenSource SharedExpiryTokenSource()
    {
        if (tokenSource.IsCancellationRequested)
        {
            tokenSource = new CancellationTokenSource(RefreshInterval);
        }

        return tokenSource;
    }
    
    public static void Refresh() => SharedExpiryTokenSource().Cancel();

    public static string GetDocumentCacheKey(Guid key) => $"DocumentCacheKey,{key}";
}