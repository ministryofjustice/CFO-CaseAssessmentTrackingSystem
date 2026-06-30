using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components;

[Authorize(Policy = SecurityPolicies.Internal)]
public partial class SyncComponent
{
    private bool _isLoading = true;
    
    private SyncRecord[] _records = [];
    
    protected override async Task OnInitializedAsync()
    {
        try
        {
            var uow = GetNewUnitOfWork();

            var query = from p in uow.DbContext.Participants
                where p.LastSyncDate.HasValue
                group p by p.LastSyncDate!.Value.Date into g
                orderby g.Key descending
                select new SyncRecord(g.Key, g.Count());
                
            var results = await query.AsNoTracking()
                .ToArrayAsync();

            if (IsDisposed == false)
            {
                _records = results;
            }
        }
        finally
        {
            _isLoading = false;
        }
    }

    private record SyncRecord(DateTime TheDate, int RecordCount);
}
