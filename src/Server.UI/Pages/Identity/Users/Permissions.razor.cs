using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.Features.Tenants.DTOs;
using Cfo.Cats.Application.Features.Tenants.Queries;

namespace Cfo.Cats.Server.UI.Pages.Identity.Users;

public partial class Permissions
{
    bool loading;
    bool showUsers = true;
    TenantHierarchyDto? tenant;

    UserSummaryDto? selected;

    [CascadingParameter] UserProfile? UserProfile { get; set; }

    GetHierarchy.Query Query { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Query = new() { UserProfile = UserProfile! };
        await RefreshAsync();
    }

    async Task RefreshAsync()
    {
        var mediator = GetNewMediator();
        
        try
        {
            loading = true;

            var result = await mediator.Send(Query);

            if(result is not { Succeeded: true, Data: not null })
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
                return;
            }

            tenant = result.Data;
        }
        finally
        {
            loading = false;
        }
    }

    async Task OnSelect()
    {
        await Task.CompletedTask;
    }
}
