using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation.Commands;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Analytics;



public partial class Cumulatives
{
    private bool _noAccessToContracts;

    [CascadingParameter]
    public UserProfile? CurrentUser { get; set; }

    private bool _downloading = false;
    
    public int Month { get; set; } = DateTime.Now.Month;
    public int Year { get; set; } = DateTime.Now.Year;
    public ContractDto? SelectedContract { get; set; }

    [Inject] private IContractService ContractService { get; set; } = default!;


    protected override void OnInitialized() => _noAccessToContracts = CurrentUser?.Contracts is [];

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await GetNewMediator().Send(new ExportCumulativeFigures.Command()
            {
                EndDate = new DateOnly(Year, Month, DateTime.DaysInMonth(Year, Month)),
                ContractId = SelectedContract?.Id
            });

            if(result.Succeeded)
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
