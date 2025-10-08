using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.QAResults;

public partial class QAEnrolmentsResults 
{
    private IEnumerable<LocationDto> _locations = [];
    //private PaginatedData<QAEnrolmentsResultsSummaryDto>? _enrolments;
    private bool _loading;

    private bool _includeInternalNotes = false;

    //public required QAEnrolmentsResultsWithPagination.Query Model { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = default!;

    [Parameter, EditorRequired]
    public bool JustMyParticipants { get; set; } = false;
    
    [Inject]
    private ILocationService LocationService { get; set; } = default!;
        
    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = default!;

    protected override IRequest<Result<PaginatedData<QAEnrolmentsResultsSummaryDto>>> CreateQuery()
 => new QAEnrolmentsResultsWithPagination.Query()
 {
     UserProfile = CurrentUser,
     PageSize = 5,
     OrderBy = "Created",
     SortDirection = $"{SortDirection.Descending}",
     JustMyParticipants = JustMyParticipants,
     IncludeInternalNotes = _includeInternalNotes
 };

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;

        _includeInternalNotes = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.Internal)).Succeeded;

        //Model = new QAEnrolmentsResultsWithPagination.Query()
        //{
        //    UserProfile = CurrentUser,
        //    PageSize = 5,
        //    OrderBy = "Created",
        //    SortDirection = $"{SortDirection.Descending}",
        //    JustMyParticipants = JustMyParticipants,
        //    IncludeInternalNotes = _includeInternalNotes
        //};
                
        try
        {
            _loading = true;
            //_enrolments = await GetNewMediator().Send(Model);
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

    private Task OnLocationChanged() => OnRefresh();

    private Task OnExcludeEnrolmentsNotInProgress(bool excludeEnrolment)
    {
        Model.Status = excludeEnrolment ? EnrolmentStatus.EnrollingStatus : null; 
        return OnRefresh();
    }

    private void EditParticipant(string participantId)
    {
        Navigation.NavigateTo($"/pages/participants/{participantId}");
    }
}