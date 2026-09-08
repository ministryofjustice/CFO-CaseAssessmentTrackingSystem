using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public partial class Feedback
{
    private MudDateRangePicker _picker = null!;
    private bool _visualMode = true;
    private bool _includeInternalColumns;
    private DateRange _dateRange = new(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today);

    [Inject]
    public IAuthorizationService AuthorizationService { get; set; } = null!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; } = default!;

    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        _includeInternalColumns = (await AuthorizationService.AuthorizeAsync(authState.User, SecurityPolicies.Internal)).Succeeded;
    }

    private string TenantId => CurrentUser.TenantId
        ?? throw new InvalidOperationException("Current user TenantId is required.");

    private string FeedbackKey => TenantId + (_dateRange.Start?.Ticks ?? 0) + (_dateRange.End?.Ticks ?? 0);
}