using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.Features.Identity.Queries.LoginSecurity;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Pages.Users;

public partial class LoginMonitoring : CatsComponent<LoginSecurityOverviewDto>
{
    private string Title { get; set; } = "Login Monitoring";
    private int _lookbackHours = 6;

    protected override IQuery<Result<LoginSecurityOverviewDto>> CreateQuery()
        => new GetLoginSecurityOverview.Query
        {
            LookbackHours = _lookbackHours
        };

    private async Task OnLookbackChanged(int hours)
    {
        _lookbackHours = hours;
        await RefreshAsync();
    }
}
