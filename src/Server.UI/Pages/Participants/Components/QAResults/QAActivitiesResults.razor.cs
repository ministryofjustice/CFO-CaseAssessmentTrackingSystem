using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Server.UI.Pages.Activities;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.QAResults;

public partial class QAActivitiesResults
{
    private IEnumerable<LocationDto> _locations = [];
    private PaginatedData<QAActivitiesResultsSummaryDto>? _activities;
    private bool _loading;

    public required QAActivitiesResultsWithPagination.Query Model { get; set; }
    
    [Inject]
    private ILocationService LocationService { get; set; } = default!;
        
    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Model = new QAActivitiesResultsWithPagination.Query()
        {
            CurentActiveUser = CurrentUser,
            PageSize = 5,
            OrderBy = "Created",
            SortDirection = $"{SortDirection.Descending}"
        };
                
        try
        {
            _loading = true;
            _activities = await GetNewMediator().Send(Model);
            _locations = LocationService
                .GetVisibleLocations(CurrentUser.TenantId!)
                .ToList();

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
        Model.PageSize = 5;
        Model.OrderBy = "Created";
        Model.SortDirection = $"{SortDirection.Descending}";

        _activities = await GetNewMediator().Send(Model);
    }

    private Task OnPaginationChanged(int arg)
    {
        Model.PageNumber = arg;
        return OnRefresh();
    }

    private Task OnDateRangeChanged(DateRange range)
    {
        Model.CommencedStart = range.Start;
        Model.CommencedEnd = range.End;
        return OnRefresh();
    }

    private Task OnLocationChanged() => OnRefresh();

    private Task OnActivityTypesChanged(IReadOnlyCollection<ActivityType>? types)
    {
        Model.IncludeTypes = types?.ToList();
        return OnRefresh();
    }

    private Task OnExcludeNotInProgress(bool exclude)
    {
        Model.Status = exclude ? ActivityStatus.PendingStatus : null;
        return OnRefresh();
    }

    private async Task EditActivity(QAActivitiesResultsSummaryDto activity)
    {   
        var parameters = new DialogParameters<EditActivityDialog>()
        {
            {
                x => x.ActivityId, activity.Id
            }
        };

        var dialog = await DialogService.ShowAsync<EditActivityDialog>("Edit Activity/ETE", parameters, new DialogOptions
        {
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            CloseButton = true
        });
    }

    private void EditParticipant(string participantId)
    {
        Navigation.NavigateTo($"/pages/participants/{participantId}");
    }
}