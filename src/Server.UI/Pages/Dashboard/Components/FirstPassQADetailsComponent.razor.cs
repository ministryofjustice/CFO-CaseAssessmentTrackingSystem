using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.Export;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class ActivitiesQAFirstPassDetailsComponent
{
    private bool _initialising = true;
    private bool _loading = false;
    private bool _downloading;
    private MudTable<ParticipantEngagementDto> _table = new();

    [CascadingParameter] public UserProfile CurrentUser { get; set; } = default!;

    private GetParticipantsLatestEngagement.Query Query { get; set; } = default!;
    protected override Task OnInitializedAsync() => RefreshAsync();

    private async Task<TableData<ParticipantEngagementDto>> ServerReload(TableState state, CancellationToken cancellationToken)
    {
        try
        {
            _loading = true;

            Query.OrderBy = state.SortLabel ?? "EngagedOn";
            Query.PageSize = state.PageSize;
            Query.PageNumber = state.Page + 1;
            Query.SortDirection = state.SortDirection == SortDirection.Ascending
                ? SortDirection.Ascending.ToString()
                : SortDirection.Descending.ToString();

            var result = await GetNewMediator().Send(Query, CancellationToken.None);

            if (result is { Succeeded: true, Data: not null })
            {
                return new TableData<ParticipantEngagementDto> { TotalItems = result.Data.TotalItems, Items = result.Data.Items };
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Warning);
                return new TableData<ParticipantEngagementDto> { TotalItems = 0, Items = [] };
            }
        }
        finally
        {
            _loading = false;
            _initialising = false;
            StateHasChanged();
        }
    }

    private async Task RefreshAsync()
    {
        Query ??= new()
        {
            JustMyCases = true,
            CurrentUser = CurrentUser
        };

        if (IsDisposed)
        {
            return;
        }

        await _table.ReloadServerData();
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await GetNewMediator().Send(new ExportParticipantsLatestEngagement.Command()
            {
                Query = Query
            });

            if (result.Succeeded)
            {
                Snackbar.Add($"{ConstantString.ExportSuccess}", Severity.Info);
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);

        }
        catch
        {
            Snackbar.Add($"An error occurred while generating your document.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }
}
