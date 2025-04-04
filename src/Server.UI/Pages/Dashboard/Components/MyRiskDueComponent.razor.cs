using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Queries;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class MyRiskDueComponent
{
    private bool _loading = true;
    private bool _approvedOnly = true;
    private string _searchString = "";

    private GetRiskDueDashboard.Query Query { get; set; } = default!;

    [CascadingParameter] public UserProfile CurrentUser { get; set; } = default!;

    private Result<RiskDueDto[]>? Model { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        Query = new GetRiskDueDashboard.Query()
        {
            UserId = CurrentUser.UserId,
            ApprovedOnly = true,
            FuturesDays = 14
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

    private async Task OnApprovedOnlyChanged(bool value)
    {
        _approvedOnly = value;
        Query.ApprovedOnly = value;
        await OnRefresh();
    }

    private bool FilterFunc(RiskDueDto data) => FilterFunc(data, _searchString);

    private bool FilterFunc(RiskDueDto data, string searchString)
    {
        if (string.IsNullOrEmpty(searchString))
        {
            return true;
        }

        if (data.ParticipantId.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if (data.FirstName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if (data.FirstName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if ($"{data.FirstName} {data.LastName}".Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }

}