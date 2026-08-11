using ApexCharts;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Application.Features.Payments.Commands;
using Cfo.Cats.Application.Features.Payments.DTOs;
using Cfo.Cats.Application.Features.Payments.Queries;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Components.Payments;

public partial class EducationPayments
{
    private bool _loading = true;
    private bool _downloading;

    [Parameter, EditorRequired] public bool DataView { get; set; }

    [Parameter, EditorRequired] public int Month { get; set; }

    [Parameter, EditorRequired] public int Year { get; set; }

    [Parameter] public ContractDto? Contract { get; set; }

    [CascadingParameter] public UserProfile CurrentUser { get; set; } = null!;

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }

    public ApexChartOptions<EducationPaymentSummaryDto> Options => new()
    {
        Chart = new Chart
        {
            Toolbar = new Toolbar { Show = false }
        },
        Theme = new Theme
        {
            Mode = IsDarkMode ? Mode.Dark : Mode.Light
        }
    };

    private EducationPaymentDto[] _payments = [];
    private List<EducationPaymentSummaryDto> _summaryData = [];

    private GetEducationPayments.Query? _query;

    private async Task OnRefresh()
    {
        try
        {
            _loading = true;

            var mediator = GetNewMediator();

            var result = await mediator.Send(_query!);

            if (result is not { Succeeded: true })
            {
                throw new Exception(result.ErrorMessage);
            }

            _payments = result.Data?.Items ?? [];
            _summaryData = result.Data?.ContractSummary ?? [];

        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally { _loading = false; }
    }

    protected override async Task OnInitializedAsync()
    {
        _query = new()
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
        _query!.Keyword = _searchString;
        await OnRefresh();
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await GetNewMediator().Send(new ExportEducationPayments.Command()
            {
                Query = _query!
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