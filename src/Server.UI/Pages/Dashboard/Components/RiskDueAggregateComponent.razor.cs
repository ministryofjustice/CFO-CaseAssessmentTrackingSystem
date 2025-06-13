using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Export;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class RiskDueAggregateComponent
{
    private bool _loading = true;
    private bool _byPerson = false;
    private string _searchString = "";
    private bool _downloading = false;

    private GetRiskDueAggregate.Query Query { get; set; } = default!;

    [CascadingParameter] public UserProfile CurrentUser { get; set; } = default!;

    private Result<RiskDueAggregateDto[]>? Model { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        Query = new GetRiskDueAggregate.Query()
        {
            TenantId = CurrentUser.TenantId!,
            GroupingType = GetRiskDueAggregate.RiskAggregateGroupingType.Tenant
        };
        await OnRefresh();
    }

    private async Task OnRefresh()
    {
        _loading = true;
        var mediator = GetNewMediator();
        Model = await mediator.Send(Query);
        _loading = false;
    }

    private async Task OnByPersonChanged(bool value)
    {
        _byPerson = value;
        Query.GroupingType =
            value
                ? GetRiskDueAggregate.RiskAggregateGroupingType.User
                : GetRiskDueAggregate.RiskAggregateGroupingType.Tenant;
        await OnRefresh();
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
            var result = await GetNewMediator().Send(new ExportRiskDueAggregate.Command()
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