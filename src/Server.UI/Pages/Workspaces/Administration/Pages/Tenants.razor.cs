using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Tenants.Commands;
using Cfo.Cats.Application.Features.Tenants.DTOs;
using Cfo.Cats.Application.Features.Tenants.Queries.GetAll;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Dialogs;
using Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components.Tenants;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Pages;

public partial class Tenants
{
    [CascadingParameter] private UserProfile? UserProfile { get; set; }
    private TenantDto? SelectedTenant { get; set; }
    private bool _loading;
    private GetAllTenantsQuery Query { get; } = new();
    private TenantDto[] Data { get; set; } = [];

    protected override async Task OnInitializedAsync() => await LoadData();

    private void OnSelectedTenantChanged(TenantDto? tenant)
    {
        SelectedTenant = tenant;
        StateHasChanged();
    }

    private IEnumerable<TenantDto> GetRootNodes()
    {
        var minDepth = Data.Min(n => n.Id.Count(c => c == '.'));
        return Data.Where(n => n.Id.Count(c => c == '.') == minDepth);
    }

    private async Task LoadData()
    {
        _loading = true;
        await Task.CompletedTask; // get tenants

        Query.UserProfile = UserProfile;

        var result = await GetNewMediator().Send(Query);

        if (result.Succeeded)
        {
            Data = result.Data!.ToArray();
            StateHasChanged();
        }

        _loading = false;
    }

    private RenderFragment RenderChildNodes(string parentId) => builder =>
    {
        var childNodes = Data.Where(n =>
            n.Id.StartsWith(parentId) && n.Id.Count(c => c == '.') == parentId.Count(c => c == '.') + 1);

        foreach (var child in childNodes)
        {
            builder.OpenComponent<MudTreeViewItem<TenantDto>>(0);
            builder.AddAttribute(1, "Text", child.Name);
            if (child.Id.Count(c => c == '.') < 3)
            {
                builder.AddAttribute(2, "Expanded", true);
            }
            else
            {
                builder.AddAttribute(2, "Expanded", false);
            }

            builder.AddAttribute(3, "Value", child);
            if (HasChildren(child))
            {
                builder.AddAttribute(4, "ChildContent",
                    (RenderFragment)((childBuilder) => { RenderChildNodes(child.Id)(childBuilder); }));
            }

            builder.CloseComponent();
        }
    };

    private bool HasChildren(TenantDto node) => Data.Any(n => n.Id.StartsWith(node.Id) && n.Id.Count(c => c == '.') == node.Id.Count(c => c == '.') + 1);

    private async Task OnAddDomain(TenantDto dto)
    {
        var parameters = new DialogParameters<AddDomainDialog>
        {
            { x => x.Model, new AddDomainCommand.Command() { TenantId = dto.Id } }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

        var dialog = await DialogService.ShowAsync<AddDomainDialog>
            (L["Add a domain to tenant allowlist"], parameters, options);

        var state = await dialog.Result;

        if (!state!.Canceled)
        {
            TenantService.Refresh();
            await LoadData();
        }
    }

    private async Task OnDeleteDomain(string domain, string tenantId)
    {
        var deleteContent = ConstantString.DeleteConfirmation;

        var parameters = new DialogParameters<DeleteConfirmation>
        {
            { x => x.Command, new DeleteDomainCommand.Command() { Domain = domain, TenantId = tenantId } },
            { x => x.ContentText, string.Format(deleteContent, domain) }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

        var dialog = await DialogService.ShowAsync<DeleteConfirmation>(L["Delete the Domain"], parameters, options);

        var state = await dialog.Result;

        if (!state!.Canceled)
        {
            TenantService.Refresh();
            await LoadData();
        }
    }

}
