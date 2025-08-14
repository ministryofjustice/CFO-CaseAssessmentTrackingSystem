using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Server.UI.Pages.Activities;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.QAResults;

public partial class QAActivitiesResults
{
    IEnumerable<LocationDto> locations = [];
    PaginatedData<QAActivitiesResultsSummaryDto>? activities;
    bool _loading;

    public required QAActivitiesResultsWithPagination.Query Model { get; set; }
    
    [Inject]
    private ILocationService LocationService { get; set; } = default!;

    [Inject]
    private ICurrentUserService CurrentUser { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Model = new QAActivitiesResultsWithPagination.Query()
        {
            CurrentUser = CurrentUser.UserId!,
            PageSize = 5,
            OrderBy = "Created",
            SortDirection = $"{SortDirection.Descending}"
        };
                
        try
        {
            _loading = true;
            activities = await GetNewMediator().Send(Model);
            locations = LocationService
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

    async Task OnRefresh()
    {
        Model.PageSize = 5;
        Model.OrderBy = "Created";
        Model.SortDirection = $"{SortDirection.Descending}";

        activities = await GetNewMediator().Send(Model);
    }

    Task OnPaginationChanged(int arg)
    {
        Model.PageNumber = arg;
        return OnRefresh();
    }

    Task OnDateRangeChanged(DateRange range)
    {
        Model.CommencedStart = range.Start;
        Model.CommencedEnd = range.End;
        return OnRefresh();
    }

    Task OnLocationChanged() => OnRefresh();

    Task OnActivityTypesChanged(IReadOnlyCollection<ActivityType>? types)
    {
        Model.IncludeTypes = types?.ToList();
        return OnRefresh();
    }

    Task OnExcludeNotInProgress(bool exclude)
    {
        Model.Status = exclude ? ActivityStatus.PendingStatus : null;
        return OnRefresh();
    }

    async Task EditActivity(QAActivitiesResultsSummaryDto activity)
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