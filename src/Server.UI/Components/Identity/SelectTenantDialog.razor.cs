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

    [Parameter]
    public bool ShowAllOption { get; set; }

    [Parameter]
    public string AllOptionLabel { get; set; } = "All Tenants";

    private bool _hasSelection = false;

    private SelectedTenant SelectedTenant { get; set; } = new(string.Empty, string.Empty);

    private void Submit()
    {
        saving = true;
        Dialog.Close(DialogResult.Ok(SelectedTenant));
    }

    private void OnTenantSelectedChanged(TenantDto? dto)
    {
        SelectedTenant = new SelectedTenant(dto?.Id ?? string.Empty, dto?.Name ?? string.Empty);
        _hasSelection = true;
    }
}

public record SelectedTenant(string TenantId, string DisplayName);