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
    private bool _includeInternalNotes = false;
    private int _pageNumber = 1;
    private LocationDto? _selectedLocation = null;
    private EnrolmentStatus? _status = null;

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = default!;

    [Parameter, EditorRequired]
    public bool JustMyParticipants { get; set; } = false;

    protected override IRequest<Result<PaginatedData<QAEnrolmentsResultsSummaryDto>>> CreateQuery() 
        => new QAEnrolmentsResultsWithPagination.Query()
        {
            UserProfile = CurrentUser,
            PageSize = 5,
            OrderBy = "Created",
            SortDirection = $"{SortDirection.Descending}",
            JustMyParticipants = JustMyParticipants,
            IncludeInternalNotes = _includeInternalNotes,
            PageNumber = _pageNumber,
            Location = _selectedLocation,
            Status = _status,
        };

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;

        _includeInternalNotes = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.Internal)).Succeeded;
        
        await LoadDataAsync();
    }

    private Task OnPaginationChanged(int arg)
    {
        _pageNumber = arg;
        return LoadDataAsync();
    }

    private Task OnExcludeEnrolmentsNotInProgress(bool excludeEnrolment)
    {
        _status = excludeEnrolment ? EnrolmentStatus.EnrollingStatus : null; 
        return LoadDataAsync();
    }

    private void EditParticipant(string participantId) => Navigation.NavigateTo($"/pages/participants/{participantId}");
}