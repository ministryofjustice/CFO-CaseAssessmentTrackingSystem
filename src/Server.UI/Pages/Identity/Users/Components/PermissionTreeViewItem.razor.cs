using Cfo.Cats.Application.Features.Tenants.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Identity.Users.Components;

public partial class PermissionTreeViewItem
{
    [Parameter, EditorRequired]
    public required TenantHierarchyDto Tenant { get; set; }

    [Parameter]
    public bool ShowUsers { get; set; } = true;
}
