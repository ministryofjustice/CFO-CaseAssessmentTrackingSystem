using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Tenants.DTOs;

namespace Cfo.Cats.Server.UI.Components.Identity;

public partial class SelectTenantDialog
{
    private bool saving = false;

    [CascadingParameter]
    private IMudDialogInstance Dialog { get; set; } = null!;

    [Parameter]
    public UserProfile CurrentUser { get; set; } = null!;

    private SelectedTenant SelectedTenant { get; set; } = new SelectedTenant(string.Empty, string.Empty);

    private void Submit()
    {
        saving = true;
        Dialog.Close(DialogResult.Ok(SelectedTenant));
    }

    private void OnTenantSelectedChanged(TenantDto? dto)
    {
        SelectedTenant = SelectedTenant with
        {
            TenantId = dto?.Id ?? string.Empty,
            DisplayName = dto?.Name ?? string.Empty
        };
    }
}

public record SelectedTenant(string TenantId, string DisplayName);