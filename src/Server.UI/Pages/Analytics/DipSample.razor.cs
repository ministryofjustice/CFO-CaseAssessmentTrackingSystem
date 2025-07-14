using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation.Queries;

namespace Cfo.Cats.Server.UI.Pages.Analytics;

public partial class DipSample
{
    private bool _loading;
    private MudDataGrid<DipSampleParticipantSummaryDto> _table = new();

    [Parameter]
    public required Guid SampleId { get; set; }

    GetDipSampleParticipants.Query Query { get; set; } = default!;

    DipSampleSummaryDto sample = default!;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _loading = true;

            Query = new()
            {
                DipSampleId = SampleId
            };

            var result = await GetNewMediator().Send(new GetDipSample.Query()
            {
                DipSampleId = SampleId
            });

            if (result is not { Succeeded: true, Data: not null })
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
                return;
            }

            sample = result.Data;
        }
        finally
        {
            _loading = false;
            await base.OnInitializedAsync();
        }
    }

    private async Task<GridData<DipSampleParticipantSummaryDto>> ServerReload(GridState<DipSampleParticipantSummaryDto> state)
    {
        try
        {
            _loading = true;

            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "ParticipantId";
            Query.PageSize = state.PageSize;
            Query.PageNumber = state.Page + 1;
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true
                ? SortDirection.Descending.ToString()
                : SortDirection.Ascending.ToString();

            var result = await GetNewMediator().Send(Query);

            if (result is { Succeeded: true, Data: not null })
            {
                return new GridData<DipSampleParticipantSummaryDto> { TotalItems = result.Data.TotalItems, Items = result.Data.Items };
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Warning);
                return new GridData<DipSampleParticipantSummaryDto> { TotalItems = 0, Items = [] };
            }
        }
        finally
        {
            _loading = false;
        }
    }

    async Task RefreshAsync() => await _table.ReloadServerData();
}
