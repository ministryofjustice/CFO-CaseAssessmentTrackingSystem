using System.Security.Cryptography;
using ActualLab.Fusion;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using RabbitMQ.Client;

namespace Cfo.Cats.Server.UI.Services;

public abstract class CatsSessionStorage<TItem>(ProtectedSessionStorage protectedSessionStorage, ICurrentUserService currentUserService)
{
    protected virtual string Key { get; } = typeof(TItem).Name;

    public async Task SetAsync(TItem item)
    {
        var itemToStore = new CatsSessionItem()
        {
            Item = item,
            SessionUserId = currentUserService.UserId!
        };
        await protectedSessionStorage.SetAsync(Key, itemToStore);
    }

    public async Task<Result<TItem>> GetAsync()
    {
        var result = await protectedSessionStorage.GetAsync<CatsSessionItem>(Key);

        if (result is { Success: true, Value.Item: not null } )
        {
            if (result.Value.SessionUserId == currentUserService.UserId)
            {
                return Result<TItem>.Success(result.Value.Item);
            }
        }

        return Result<TItem>.Failure();
    }
    
    
    private class CatsSessionItem
    {
        /// <summary>
        /// The UserId of the person who is stored this data
        /// </summary>
        public required string SessionUserId { get; set; }
    
        public required TItem Item { get; set; }
    }

}
