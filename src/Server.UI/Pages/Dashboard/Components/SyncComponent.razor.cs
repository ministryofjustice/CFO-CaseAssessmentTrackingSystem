using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class SyncComponent
{

    private bool _isLoading = true;
    
    private SyncRecord[] _records = [];
    
    protected override async Task OnInitializedAsync()
    {

        try
        {
            var uow = GetNewUnitOfWork();

            var query = from d in uow.DbContext.DateDimensions
                join p in uow.DbContext.Participants
                    on true equals true// placeholder for where join is done in `where`
                where p.LastSyncDate >= d.TheDate &&
                      p.LastSyncDate < d.TheDate.AddDays(1)
                group p by d.TheDate
                into g
                orderby g.Key
                select new SyncRecord(g.Key, g.Count());
                
        
            var results = await query.ToArrayAsync();
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

    public record SyncRecord(DateTime TheDate, int RecordCount);
}
