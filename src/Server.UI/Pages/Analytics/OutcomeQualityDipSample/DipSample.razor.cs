using Cfo.Cats.Application.Features.ManagementInformation.Commands;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation.Queries;

namespace Cfo.Cats.Server.UI.Pages.Analytics.OutcomeQualityDipSample;

public partial class DipSample
{
    private bool _loading;
    private bool _downloading;
    private MudDataGrid<DipSampleParticipantSummaryDto> _table = new();

    private List<BreadcrumbItem> Items =>
    [
        new("Outcome Quality", href: "/pages/analytics/outcome-quality-dip-sampling/", icon: Icons.Material.Filled.Home),
        new($"{_sample?.ContractName} ({_sample?.PeriodFromDesc})", href: $"/pages/analytics/outcome-quality-dip-sampling/{SampleId}", icon: Icons.Material.Filled.List)
    ];

    [Parameter]
    public required Guid SampleId { get; set; }

    private GetOutcomeQualityDipSampleParticipants.Query Query { get; set; } = default!;

    private DipSampleSummaryDto? _sample;

    protected override Task OnInitializedAsync() => RefreshAsync();

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

    private async Task RefreshAsync()
    {
        Query = new()
        {
            DipSampleId = SampleId
        };

        var result = await GetNewMediator().Send(new GetOutcomeQualityDipSample.Query()
        {
            DipSampleId = SampleId
        });

        if(IsDisposed)
        {
            return;
        }

        if (result is not { Succeeded: true, Data: not null })
        {
            Snackbar.Add(result.ErrorMessage, Severity.Error);
            return;
        }

        _sample = result.Data;
        await _table.ReloadServerData();
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            await Task.CompletedTask;
        }
        finally
        {
            _downloading = false;
        }
    }

    private async Task OnReview()
    {
        try
        {
                _loading = true;

            var command = new Application.Features.ManagementInformation.Commands.ReviewOutcomeQualityDipSample.Command() { SampleId = SampleId };
            var result = await GetNewMediator().Send(command);

            if (IsDisposed)
            {
                return;
            }

            if (result is { Succeeded: false })
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
            else
            {
                Snackbar.Add("Review submitted");
            }
            await RefreshAsync();
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task OnVerify()
    {
        try
        {
            _loading = true;

            var command = new VerifyOutcomeQualityDipSample.Command()
            {
                SampleId = SampleId
            };

            var result = await GetNewMediator().Send(command);

            if (IsDisposed)
            {
                return;
            }

            if (result is { Succeeded: false })
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
            else
            {
                Snackbar.Add("Verification submitted.");
            }
            await RefreshAsync();
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task OnFinalise()
    {
        try
        {
            _loading = true;

            await Task.CompletedTask;
        }
        finally
        {
            _loading = false;
        }
    }
}
