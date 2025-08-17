using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Queries;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class RiskDue
{
    private bool _loading = true;

    private RiskDueDto[] Model { get; set; } = [];

    private bool _approvedOnly = true;

    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = default!;

    [Inject] private IUserService UserService { get; set; } = default!;

    protected override Task OnInitializedAsync()
    {
        return Refresh();
    }

    private async Task Refresh()
    {
        _loading = true;

        var query = new GetRiskDueDashboard.Query()
        {
            UserId = CurrentUser.UserId,
            FuturesDays = 14,
            ApprovedOnly = _approvedOnly
        };

        var mediator = GetNewMediator();

        Model = await mediator.Send(query);

        _loading = false;

    }
    
}