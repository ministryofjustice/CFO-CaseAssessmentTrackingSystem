using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;


namespace Cfo.Cats.Server.UI.Pages.Dashboard;

[Authorize(Policy = SecurityPolicies.AuthorizedUser)]
public partial class Dashboard
{

    private bool _showQaPots;
    private bool _showMyTeamsParticipants;
    private bool _showJobManagement;
    private bool _showCaseWorkload;
    private bool _showRiskDueAggregate;
    private bool _showSyncComponent;

    public string Title { get; } = "Dashboard";


    [CascadingParameter] private UserProfile UserProfile { get; set; } = default!;

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var transaction = SentrySdk.StartTransaction("Dashboard", "page.load");
        SentrySdk.ConfigureScope(scope => scope.Transaction = transaction);

        try
        {
            var state = await AuthState;

            _showMyTeamsParticipants =
                (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.UserHasAdditionalRoles)).Succeeded;

            // these  follow the same logic for now no need to make the same check.
            _showCaseWorkload = _showMyTeamsParticipants;
            _showRiskDueAggregate = _showCaseWorkload;

            _showQaPots = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.Internal)).Succeeded;

            // this follows the same check as QA pots
            _showSyncComponent = _showQaPots;

            _showJobManagement = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.SystemSupportFunctions))
                .Succeeded;
        }
        catch (Exception ex)
        {
            transaction.Finish(SpanStatus.InternalError);
            SentrySdk.CaptureException(ex);
            throw;
        }
        finally
        {
            if (transaction.IsFinished == false)
            {
                transaction.Finish();
            }
            SentrySdk.ConfigureScope(scope => scope.Transaction = null);
        }
    }

  
}