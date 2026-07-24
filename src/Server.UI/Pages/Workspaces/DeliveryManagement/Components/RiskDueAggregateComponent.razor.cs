using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Export;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Components;

public partial class RiskDueAggregateComponent : CatsComponent<RiskDueAggregateDto[]>
{
    private bool _byPerson = false;
    private string _searchString = "";
    private bool _downloading = false;

    private GetRiskDueAggregate.Query Query { get; set; } = default!;

    protected override IQuery<Result<RiskDueAggregateDto[]>> CreateQuery()
    {
        Query = new GetRiskDueAggregate.Query()
        {
            TenantId = CurrentUser.TenantId!,
            GroupingType = _byPerson
                ? GetRiskDueAggregate.RiskAggregateGroupingType.User
                : GetRiskDueAggregate.RiskAggregateGroupingType.Tenant
        };
        return Query;
    }

    private async Task OnByPersonChanged(bool value)
    {
        _byPerson = value;
        await RefreshAsync();
    }

    private bool FilterFunc(RiskDueAggregateDto data) => FilterFunc(data, _searchString);

    private bool FilterFunc(RiskDueAggregateDto data, string searchString)
    {
        if (string.IsNullOrEmpty(searchString))
        {
            return true;
        }

        if (data.Description.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await Service.Send(new ExportRiskDueAggregate.Command()
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