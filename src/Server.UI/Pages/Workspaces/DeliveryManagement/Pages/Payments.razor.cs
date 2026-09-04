using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public partial class Payments
{
    private bool _noAccessToContracts;
    
    [CascadingParameter] 
    public UserProfile CurrentUser { get; set; } = null!;

    public int Month { get; set; } = DateTime.Now.Month;
    public int Year { get; set; } = DateTime.Now.Year;
    public bool VisualMode { get; set; } = true;
    public ContractDto? SelectedContract { get; set; }

    [Inject] private IContractService ContractService { get; set; } = null!;

    protected override void OnInitialized() => _noAccessToContracts = CurrentUser.Contracts is [];

    private void OnMonthChanged(int month) => Month = month;
    private void OnYearChanged(int year) => Year = year;

    private void OnContractChanged(ContractDto contract) => SelectedContract = contract;

    private void ClearSearch()
    {
        Month = DateTime.Now.Month;
        Year = DateTime.Now.Year;
        SelectedContract = null;
    }

}