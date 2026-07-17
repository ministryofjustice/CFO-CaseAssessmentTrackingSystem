using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Features.Participants.Specifications;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Components;

public partial class ParticipantFilterBar
{
    [Parameter, EditorRequired]
    public ParticipantsWithPagination.Query Query { get; set; } = null!;

    [Parameter]
    public bool CanReassign { get; set; }

    [Parameter]
    public int SelectedCount { get; set; }

    [Parameter]
    public int TotalPages { get; set; }

    [Parameter]
    public int TotalItems { get; set; }

    [Parameter]
    public bool? Tabular { get; set; }

    [Parameter]
    public bool IsDownloading { get; set; }

    [Parameter, EditorRequired]
    public IDictionary<string, string> Users { get; set; } = null!;

    [Parameter, EditorRequired]
    public IDictionary<string, string> Tenants { get; set; } = null!;

    [Parameter, EditorRequired]
    public IDictionary<int, string> Locations { get; set; } = null!;

    [Parameter, EditorRequired]
    public IDictionary<Guid, LabelDto> Labels { get; set; } = null!;

    [Parameter, EditorRequired]
    public string CurrentFilterLabel { get; set; } = null!;

    [Parameter]
    public EventCallback OnClearQuickFilter { get; set; }

    [Parameter]
    public EventCallback OnMyParticipants { get; set; }

    [Parameter]
    public EventCallback OnOverdueRisk { get; set; }

    [Parameter]
    public EventCallback<RecentParticipantFilter> OnRecentlyAssigned { get; set; }

    [Parameter]
    public EventCallback OnRecentlyVisited { get; set; }

    [Parameter]
    public EventCallback<string?> OnSearchChanged { get; set; }

    [Parameter]
    public EventCallback OnReassignSelected { get; set; }

    [Parameter]
    public EventCallback OnClearSelection { get; set; }

    [Parameter]
    public EventCallback OnRefresh { get; set; }

    [Parameter]
    public EventCallback OnExport { get; set; }

    [Parameter]
    public EventCallback OnEnrol { get; set; }

    [Parameter]
    public EventCallback<bool?> OnTabularChanged { get; set; }

    [Parameter]
    public EventCallback<ParticipantListView> OnListViewChanged { get; set; }

    [Parameter]
    public EventCallback OnShowAssigneeDialog { get; set; }

    [Parameter]
    public EventCallback OnShowTenantDialog { get; set; }

    [Parameter]
    public EventCallback OnShowLocationDialog { get; set; }

    [Parameter]
    public EventCallback<LabelDto?> OnLabelChanged { get; set; }

    [Parameter]
    public EventCallback<string> OnSortBy { get; set; }

    [Parameter]
    public EventCallback OnClearSearch { get; set; }

    private string GetCurrentFilterLabel() => CurrentFilterLabel;
}
