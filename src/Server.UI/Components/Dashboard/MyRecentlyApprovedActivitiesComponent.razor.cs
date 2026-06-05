using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class MyRecentlyApprovedActivitiesComponent : CatsComponent<PaginatedData<RecentlyApprovedActivitiesSummaryDto>>
{
    [EditorRequired, Parameter]
    public DateRange? DateRange { get; set; }

    [Parameter]
    public string UserId { get; set; } = null!;

    [Parameter]
    public string TenantId { get; set; } = null!;

    [EditorRequired, Parameter]
    public bool VisualMode { get; set; }

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }
    
    private int _pageNumber = 1;
    private LocationDto? _location;
    private List<ActivityType>? _includeTypes;

    protected override IRequest<Result<PaginatedData<RecentlyApprovedActivitiesSummaryDto>>> CreateQuery()
        => new GetRecentlyApprovedActivities.Query()
        {
            UserProfile = CurrentUser,
            PageSize = 10,
            OrderBy = "ApprovedOn",
            SortDirection = $"{SortDirection.Descending}",
            PageNumber = _pageNumber,
            Location = _location,
            ApprovedStart = DateRange?.Start ?? throw new InvalidOperationException("DateRange not set"),
            ApprovedEnd = DateRange?.End ?? throw new InvalidOperationException("DateRange not set"),
            IncludeTypes = _includeTypes,
            TenantId = TenantId
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

    private Task OnActivityTypesChanged(IReadOnlyCollection<ActivityType>? types)
    {
        _includeTypes = types?.ToList();
        return OnRefresh();
    }

    private Task OnLocationChanged() => OnRefresh();

    private void EditParticipant(string participantId) => Navigation.NavigateTo($"/pages/participants/{participantId}");
}
