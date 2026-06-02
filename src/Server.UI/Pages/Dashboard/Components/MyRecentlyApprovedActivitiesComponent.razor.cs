using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Domain.Common.Enums;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class MyRecentlyApprovedActivitiesComponent
{
    private List<RecentlyApprovedActivitiesSummaryDto>? _activities;
    private bool _loading;
    private LocationDto? _location;

    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = default!;

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = default!;

    private GetRecentlyApprovedActivities.Query _query = default!;

    protected override async Task OnInitializedAsync()
    {
        _query = new GetRecentlyApprovedActivities.Query()
        {
            UserProfile = CurrentUser,
            DaysBack = 30
        };

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
        _query.Location = _location;
        _activities = await GetNewMediator().Send(_query);
    }

    private Task OnDateRangeChanged(DateRange? range)
    {
        _query.CommencedStart = range?.Start;
        _query.CommencedEnd = range?.End;
        return OnRefresh();
    }

    private Task OnActivityTypesChanged(IReadOnlyCollection<ActivityType>? types)
    {
        _query.IncludeTypes = types?.ToList();
        return OnRefresh();
    }

    private void EditParticipant(string participantId) => Navigation.NavigateTo($"/pages/participants/{participantId}");
}
