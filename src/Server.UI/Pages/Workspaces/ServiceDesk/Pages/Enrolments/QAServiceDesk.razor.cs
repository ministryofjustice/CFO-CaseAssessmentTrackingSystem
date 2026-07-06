using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Components;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.ServiceDesk.Pages.Enrolments;

public partial class QAServiceDesk
{
    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = null!;

    [Parameter]
    [SupplyParameterFromQuery(Name = "tab")]
    public string? Tab { get; set; }

    private int _activePanelIndex;

    protected override void OnParametersSet()
    {
        var canViewManagedQueues = CurrentUser.AssignedRoles.Any(r =>
            r == RoleNames.QAManager ||
            r == RoleNames.QASupportManager ||
            r == RoleNames.SMT ||
            r == RoleNames.SystemSupport);

        _activePanelIndex = Tab?.ToLowerInvariant() switch
        {
            "second-pass" when canViewManagedQueues => 1,
            "escalation" when canViewManagedQueues => 2,
            _ => 0
        };
    }
}
