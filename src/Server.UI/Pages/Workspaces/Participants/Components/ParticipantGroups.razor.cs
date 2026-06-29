using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Features.Participants.Specifications;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Components;

public partial class ParticipantGroups
{
    [Inject]
    private IMediator Mediator { get; set; } = null!;

    /// <summary>The group headers (key/label/count). Rows are fetched on demand.</summary>
    [Parameter, EditorRequired]
    public ParticipantGroupDto[] Groups { get; set; } = [];

    /// <summary>The active filter, used to scope each group's lazy fetch.</summary>
    [Parameter, EditorRequired]
    public ParticipantFilter Filter { get; set; } = null!;

    [Parameter]
    public bool CanReassign { get; set; }

    [Parameter]
    public int RowsPerGroup { get; set; } = 50;

    [Parameter]
    public HashSet<string> SelectedIds { get; set; } = [];

    [Parameter]
    public EventCallback<HashSet<string>> SelectedIdsChanged { get; set; }

    private ParticipantGroupView[] _groups = [];

    protected override void OnParametersSet()
        => _groups = Groups.Select(g => new ParticipantGroupView(g)).ToArray();

    private async Task OnGroupExpanded(ParticipantGroupView group, bool expanded)
    {
        if (!expanded || group.Loaded || group.Loading)
        {
            return;
        }

        group.Loading = true;

        var result = await Mediator.Send(BuildGroupQuery(group.Key));
        group.Items = result is { Succeeded: true, Data: not null } ? result.Data.Items.ToArray() : [];
        group.Loaded = true;
        group.Loading = false;
    }

    // Fetches a single group's participants by reusing the flat query, scoped to the group key.
    private ParticipantsWithPagination.Query BuildGroupQuery(string key) => new(Filter)
    {
        GroupBy = ParticipantGroupBy.None,
        OwnerId = Filter.GroupBy == ParticipantGroupBy.Assignee ? key : Filter.OwnerId,
        TenantId = Filter.GroupBy == ParticipantGroupBy.Tenant ? key : Filter.TenantId,
        PageNumber = 1,
        PageSize = RowsPerGroup
    };

    private sealed class ParticipantGroupView(ParticipantGroupDto group)
    {
        public string Key => group.Key;
        public string Label => group.Label;
        public int Count => group.Count;
        public ParticipantPaginationDto[] Items { get; set; } = [];
        public bool Loaded { get; set; }
        public bool Loading { get; set; }
    }
}
