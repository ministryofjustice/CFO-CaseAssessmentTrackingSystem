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

    private CumulativeFiguresDto? _results;
    private ContractTargetDto? _targets;

    [CascadingParameter] public UserProfile CurrentUser { get; set; } = default!;
    [Parameter] public ContractDto? Contract { get; set; }

    [Parameter, EditorRequired]
    public DateOnly EndDate { get; set; }

    
    protected override async Task OnInitializedAsync()
    {
        await LoadActual();
        LoadTargets();
        _isLoading = false;
    }

    private void LoadTargets()
    {
        //start date is fixed
        var startDate = new DateOnly(2025, 1, 1);

        var monthlyDates = Enumerable.Range(0, int.MaxValue)
            .Select(i => startDate.AddMonths(i))
            .TakeWhile(date => date <= EndDate)
            .ToArray();

        // get all contracts that apply

        string[] contracts = this.Contract is null ? 
            ContractService.GetVisibleContracts(CurrentUser.TenantId!)
            .Select(contract => contract.Name)
            .ToArray()
            : [this.Contract.Name];
        
        // iterate over every contract and get the targets for the months
        ContractTargetDto target = new ContractTargetDto("", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        foreach (var contract in contracts)
        {
            foreach (var month in monthlyDates)
            {
                target += TargetsProvider.GetTarget(contract, month.Month, month.Year);
            }
        }

        _targets = target;


    }

    private async Task LoadActual()
    {
        var mediator = GetNewMediator();
        var query = new GetCumulativeFigures.Query()
        {
            EndDate = this.EndDate
        };
        var result = await mediator.Send(query);
        if (result is { Succeeded: true, Data: not null })
        {
            if (Contract is null)
            {
                _results = result.Data!.Aggregate(
                    new CumulativeFiguresDto("All", "All", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
                    (acc, x) => acc with
                    {
                        custody_enrolments = acc.custody_enrolments + x.custody_enrolments,
                        community_enrolments = acc.community_enrolments + x.community_enrolments,
                        wing_inductions = acc.wing_inductions + x.wing_inductions,
                        hub_inductions = acc.hub_inductions + x.hub_inductions,
                        prerelease_support = acc.prerelease_support + x.prerelease_support,
                        ttg = acc.ttg + x.ttg,
                        support_work = acc.support_work + x.support_work,
                        human_citizenship = acc.human_citizenship + x.human_citizenship,
                        community_and_social = acc.community_and_social + x.community_and_social,
                        isws = acc.isws + x.isws,
                        employment = acc.employment + x.employment,
                        education = acc.education + x.education
                    }
                );
            }
            else
            {
                _results = result.Data!.First(r => r.contract_id == Contract.Id);
            }



        }

    }
}