using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Application.Features.AuditTrails.Caching;
using Cfo.Cats.Application.Features.AuditTrails.DTOs;
using Cfo.Cats.Application.Features.AuditTrails.Queries.GetSystemAuditTrailsWithPagination;
using Cfo.Cats.Application.Features.AuditTrails.Specifications.SystemAuditTrail;
using Cfo.Cats.Application.Features.PathwayPlans.Queries;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class ParticipantAudit
{
    [Parameter, EditorRequired] public string Id { get; set; } = null!;

    private readonly List<ListEntry> _entries = [];
    private AuditTrailsWithPaginationQuery Query { get; } = new();
    private MudDataGrid<AuditTrailDto> _table = null!;
    private bool _loading;
    private int _defaultPageSize = 15;
    private readonly AuditTrailDto _currentDto = new();

    [CascadingParameter] private UserProfile? UserProfile { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _entries.Add(new ListEntry()
        {
            Title = "Participant", PrimaryKey = new()
            {
                { "Id", Id }
            }
        });

        var pathwayQuery = new GetPathwayPlanByParticipantId.Query()
        {
            ParticipantId = Id
        };

        var pathwayResult = await GetNewMediator().Send(pathwayQuery);

        var activitiesQuery = new ActivitiesWithPagination.Query()
        {
            ParticipantId = Id,
            PageSize = 200
        };

        var activitiesResults = await GetNewMediator().Send(activitiesQuery);

        _entries.Add(new ListEntry()
        {
            Title = "Pathway Plan", PrimaryKey = new()
            {
                { "Id", pathwayResult!.Id }
            }
        });

        foreach (var objective in pathwayResult.Objectives)
        {
            _entries.Add(new ListEntry()
            {
                Title = $"Objective: {objective.DisplayName}",
                PrimaryKey = new()
                {
                    { "Id", objective.Id }
                }
            });

            foreach (var t in objective.Tasks)
            {
                _entries.Add(new ListEntry()
                {
                    Title = $"Task: {t.DisplayName}",
                    PrimaryKey = new()
                    {
                        { "Id", t.Id }
                    }
                });
            }
        }

        if (activitiesResults.Succeeded && activitiesResults.Data is not null)
        {
            foreach (var entry in activitiesResults.Data.Items)
            {
                _entries.Add(new ListEntry()
                {
                    Title = $"Activity: {entry.Definition} ({entry.CommencedOn})",
                    PrimaryKey = new()
                    {
                        { "Id", entry.Id }
                    }
                });
            }
        }

        Query.PrimaryKey = _entries.First().PrimaryKey;
    }

    private class ListEntry
    {
        public required string Title { get; init; }
        public required Dictionary<string, object> PrimaryKey { get; init; } = new();
    }

    private async Task OnValueChanged(ListEntry selected)
    {
        Query.PrimaryKey = selected.PrimaryKey;
        await _table.ReloadServerData();
    }

    private async Task OnChangedListView(SystemAuditTrailListView listview)
    {
        Query.ListView = listview;
        await _table.ReloadServerData();
    }

    private async Task<GridData<AuditTrailDto>> ServerReload(GridState<AuditTrailDto> state,
        CancellationToken cancellationToken)
    {
        try
        {
            _loading = true;
            Query.CurrentUser = UserProfile;
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Id";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true
                ? nameof(SortDirection.Descending)
                : nameof(SortDirection.Ascending);
            Query.PageNumber = state.Page + 1;
            Query.PageSize = state.PageSize;
            Query.PrimaryKey ??= _entries.First().PrimaryKey;

            var result = await GetNewMediator().Send(Query, cancellationToken);
            return new GridData<AuditTrailDto> { TotalItems = result.TotalItems, Items = result.Items };
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task OnSearch(AuditType? val)
    {
        Query.AuditType = val;
        await _table.ReloadServerData();
    }

    private async Task OnRefresh()
    {
        SystemAuditTrailsCacheKey.Refresh();
        Query.Keyword = string.Empty;
        await _table.ReloadServerData();
    }
}