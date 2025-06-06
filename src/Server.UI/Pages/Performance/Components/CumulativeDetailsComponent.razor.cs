using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation.Queries;

namespace Cfo.Cats.Server.UI.Pages.Performance.Components;

public partial class CumulativeDetailsComponent : CatsComponentBase
{

    [Inject] public ITargetsProvider TargetsProvider { get; set; } = default!;
    [Inject] public IContractService ContractService { get; set; } = default!;

    private bool _isLoading = true;

    private CumulativeFiguresDto? _cumulativeFigures;

    [CascadingParameter] public UserProfile CurrentUser { get; set; } = default!;
    [Parameter] public ContractDto? Contract { get; set; }

    [Parameter, EditorRequired]
    public DateOnly EndDate { get; set; }

    
    protected override async Task OnInitializedAsync()
    {
        await LoadActual();
        _isLoading = false;
    }

   
    private async Task LoadActual()
    {
        var mediator = GetNewMediator();
        var query = new GetCumulativeFigures.Query
        {
            EndDate = this.EndDate,
            ContractId = this.Contract?.Id,
            CurrentUser = CurrentUser
        };
        
        var result = await mediator.Send(query);
        if (result is { Succeeded: true, Data: not null })
        {
            _cumulativeFigures = result.Data;
        }

    }
}