using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Pages.Users;

public partial class UserFilterBar
{
    [Parameter]
    public string? SearchString { get; set; }

    [Parameter]
    public string? SelectedTenantId { get; set; }

    [Parameter]
    public string? SelectedRole { get; set; }

    [Parameter]
    public UserStatus? SelectedStatus { get; set; }

    [Parameter]
    public bool IsDownloading { get; set; }

    [Parameter]
    public bool CanCreate { get; set; }

    [Parameter]
    public int CurrentPage { get; set; }

    [Parameter]
    public int TotalPages { get; set; }

    [Parameter]
    public int TotalItems { get; set; }

    [Parameter, EditorRequired]
    public IDictionary<string, string> Tenants { get; set; } = null!;

    [Parameter, EditorRequired]
    public IEnumerable<string> Roles { get; set; } = null!;

    [Parameter]
    public EventCallback<string?> OnSearchChanged { get; set; }

    [Parameter]
    public EventCallback OnRefresh { get; set; }

    [Parameter]
    public EventCallback OnExport { get; set; }

    [Parameter]
    public EventCallback OnCreate { get; set; }

    [Parameter]
    public EventCallback OnShowTenantDialog { get; set; }

    [Parameter]
    public EventCallback OnShowRoleDialog { get; set; }

    [Parameter]
    public EventCallback<UserStatus?> OnStatusChanged { get; set; }

    [Parameter]
    public EventCallback OnClearSearch { get; set; }

    [Parameter]
    public EventCallback<int> OnPageChanged { get; set; }

    private string GetStatusLabel() => SelectedStatus is null ? "All Statuses" : SelectedStatus.DisplayName;
}
