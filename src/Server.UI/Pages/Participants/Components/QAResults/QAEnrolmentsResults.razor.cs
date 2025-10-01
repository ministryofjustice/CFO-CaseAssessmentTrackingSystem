using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.Queries;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.QAResults;

public partial class QAEnrolmentsResults 
{
    private IEnumerable<LocationDto> _locations = [];
    private PaginatedData<QAEnrolmentsResultsSummaryDto>? _enrolments;
    private bool _loading;

    public required QAEnrolmentsResultsWithPagination.Query Model { get; set; }

    [Parameter, EditorRequired]
    public bool JustMyParticipants { get; set; } = false;
    
    [Inject]
    private ILocationService LocationService { get; set; } = default!;
        
    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Model = new QAEnrolmentsResultsWithPagination.Query()
        {
            UserProfile = CurrentUser,
            PageSize = 5,
            OrderBy = "Created",
            SortDirection = $"{SortDirection.Descending}",
            JustMyParticipants = JustMyParticipants
        };
                
        try
        {
            _loading = true;
            _enrolments = await GetNewMediator().Send(Model);
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

        _enrolments = await GetNewMediator().Send(Model);
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
       
    //private Task OnExcludeNotInProgress(bool exclude)
    //{
    //    Model.Status = exclude ? EnrolmentStatus.ApprovedStatus : null;
    //    return OnRefresh();
    //}

    private void EditParticipant(string participantId)
    {
        Navigation.NavigateTo($"/pages/participants/{participantId}");
    }
}