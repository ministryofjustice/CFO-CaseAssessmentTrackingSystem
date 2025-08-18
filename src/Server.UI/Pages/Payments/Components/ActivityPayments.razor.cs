using ApexCharts;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Application.Features.Payments.Commands;
using Cfo.Cats.Application.Features.Payments.DTOs;
using Cfo.Cats.Application.Features.Payments.Queries;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Payments.Components;

public partial class ActivityPayments
{
    private readonly ApexChartOptions<ActivityPaymentSummaryDto> _options = new();
    private bool _loading;
    private bool _downloading;

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }

    [Parameter, EditorRequired] public bool DataView { get; set; }

    [Parameter, EditorRequired] public int Month { get; set; }

    [Parameter, EditorRequired] public int Year { get; set; }

    [Parameter] public ContractDto? Contract { get; set; }

    [CascadingParameter] public UserProfile CurrentUser { get; set; } = default!;

    public ApexChartOptions<ActivityPaymentSummaryDto> Options => new()
    {
        Theme = new Theme
        {
            Mode = IsDarkMode ? Mode.Dark : Mode.Light
        }
    };

    private ActivityPaymentDto[] Payments = [];
    private List<ActivityPaymentSummaryDto> SummaryData = [];

    private GetActivityPayments.Query? Query;

    private async Task OnRefresh()
    {
        try
        {
            _loading = true;

            var mediator = GetNewMediator();

            var result = await mediator.Send(Query!);

            if (result is not { Succeeded: true })
            {
                throw new Exception(result.ErrorMessage);
            }

            Payments = result.Data?.Items ?? [];
            SummaryData = result.Data?.ContractSummary ?? [];

        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally { _loading = false; }
    }

    protected override async Task OnInitializedAsync()
    {
        Query = new()
        {
            ContractId = Contract?.Id,
            Month = Month,
            Year = Year,
            TenantId = CurrentUser!.TenantId!
        };

        await OnRefresh();
    }

    private string _searchString = "";

    private async Task OnSearch()
    {
        Query!.Keyword = _searchString;
        await OnRefresh();
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await GetNewMediator().Send(new ExportActivityPayments.Command()
            {
                Query = Query!
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