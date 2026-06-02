using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class MyRecentlyApprovedActivitiesComponent
{
    private List<RecentlyApprovedActivitiesSummaryDto>? _activities;
    private bool _loading;

    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = default!;

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _loading = true;
            await OnRefresh();
        }
        finally
        {
            _loading = false;
        }

        await base.OnInitializedAsync();
    }

    private async Task OnRefresh()
    {
        var query = new GetRecentlyApprovedActivities.Query()
        {
            UserProfile = CurrentUser,
            DaysBack = 30
        };

        _activities = await GetNewMediator().Send(query);
    }

    private void EditParticipant(string participantId) => Navigation.NavigateTo($"/pages/participants/{participantId}");
}
