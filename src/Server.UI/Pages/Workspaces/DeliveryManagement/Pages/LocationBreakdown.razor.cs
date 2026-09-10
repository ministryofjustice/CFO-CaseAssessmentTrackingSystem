using Cfo.Cats.Application.Features.Dashboard.Queries;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public partial class LocationBreakdown
{
    protected override async Task<IDictionary<string, string>> LoadUsersAsync()
    {
        var result = await GetNewMediator().Send(new GetCasesPerLocationAssignees.Query(CurrentUser)
        {
            TenantId = SelectedTenantId
        });

        return result is { Succeeded: true, Data: not null }
            ? result.Data.ToDictionary(a => a.Id, a => a.DisplayName)
            : new Dictionary<string, string>();
    }
}
