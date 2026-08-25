using System.Globalization;
using Cfo.Cats.Application.Features.ManagementInformation.ContractTargets.Commands.UpdateContractTarget;
using Cfo.Cats.Application.Features.ManagementInformation.ContractTargets.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation.ContractTargets.Queries;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components.ContractTargets;

public partial class ContractTargetsTable
{
    private int? _year = DateTime.Today.Year;
    private int? _month = DateTime.Today.Month;

    private static readonly int[] Years = Enumerable.Range(2025, 5).ToArray(); // 2025 - 2029
    private static readonly int[] Months = Enumerable.Range(1, 12).ToArray();

    protected override IQuery<Result<ContractTargetListItemDto[]>> CreateQuery()
        => new GetContractTargets.Query { Year = _year, Month = _month };

    private async Task OnYearChanged(int? year)
    {
        _year = year;
        await RefreshAsync();
    }

    private async Task OnMonthChanged(int? month)
    {
        _month = month;
        await RefreshAsync();
    }

    private static string MonthName(int month)
        => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

    private async Task OnEdit(ContractTargetListItemDto context)
    {
        var command = new UpdateContractTargetCommand
        {
            Id = context.Id,
            Prison = context.Prison,
            Community = context.Community,
            Wings = context.Wings,
            Hubs = context.Hubs,
            PreReleaseSupport = context.PreReleaseSupport,
            ThroughTheGate = context.ThroughTheGate,
            SupportWork = context.SupportWork,
            HumanCitizenship = context.HumanCitizenship,
            CommunityAndSocial = context.CommunityAndSocial,
            Interventions = context.Interventions,
            Employment = context.Employment,
            TrainingAndEducation = context.TrainingAndEducation
        };

        var parameters = new DialogParameters<EditContractTargetDialog>
        {
            { x => x.Model, command },
            { x => x.ContractName, context.ContractName},
            { x => x.Period, $"{context.MonthName}-{context.Year}" }
        };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            FullWidth = true,
            MaxWidth = MaxWidth.Medium,
            CloseOnNavigation = false
        };

        var result = await DialogService.ShowAsync<EditContractTargetDialog>("Edit Contract Target", parameters, options);
        var dialogResult = await result.Result;
        if (dialogResult!.Canceled == false)
        {
            await RefreshAsync();
        }
    }
}
