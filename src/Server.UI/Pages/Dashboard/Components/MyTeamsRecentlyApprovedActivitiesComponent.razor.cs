using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Domain.Common.Enums;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class MyTeamsRecentlyApprovedActivitiesComponent : CatsComponent<PaginatedData<RecentlyApprovedActivitiesSummaryDto>>
{
    private int _pageNumber = 1;
    private LocationDto? _location;
    private DateTime? _commencedStart;
    private DateTime? _commencedEnd;
    private List<ActivityType>? _includeTypes;

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = default!;

    protected override IRequest<Result<PaginatedData<RecentlyApprovedActivitiesSummaryDto>>> CreateQuery()
        => new GetRecentlyApprovedActivities.Query()
        {
            UserProfile = CurrentUser,
            DaysBack = 30,
            PageSize = 10,
            OrderBy = "ApprovedOn",
            SortDirection = $"{SortDirection.Descending}",
            PageNumber = _pageNumber,
            Location = _location,
            CommencedStart = _commencedStart,
            CommencedEnd = _commencedEnd,
            IncludeTypes = _includeTypes,
            JustMyParticipants = false  
        };

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
        await base.OnInitializedAsync();
    }

    private Task OnPaginationChanged(int pageNumber)
    {
        _pageNumber = pageNumber;
        return LoadDataAsync();
    }

    private Task OnRefresh()
    {
        _pageNumber = 1; 
        return LoadDataAsync();
    }

    private Task OnDateRangeChanged(DateRange? range)
    {
        _commencedStart = range?.Start;
        _commencedEnd = range?.End;
        return OnRefresh();
    }

    private Task OnActivityTypesChanged(IReadOnlyCollection<ActivityType>? types)
    {
        _includeTypes = types?.ToList();
        return OnRefresh();
    }

    private Task OnLocationChanged() => OnRefresh();

    private void EditParticipant(string participantId) => Navigation.NavigateTo($"/pages/participants/{participantId}");
}
