using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Server.UI.Components.Locations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public partial class QAEnrolmentsResults 
{

    private bool _includeInternalNotes = false;
    private int _pageNumber = 1;
    private LocationDto? _selectedLocation = null;
    private EnrolmentStatus? _status = null;
    private bool _tabular;
    private readonly HashSet<string> _expandedParticipants = [];

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = default!;

    [Parameter, EditorRequired]
    public bool JustMyParticipants { get; set; } = false;

    protected override IQuery<Result<PaginatedData<QAEnrolmentsResultsSummaryDto>>> CreateQuery() 
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
        _pageNumber = 1;
        return LoadDataAsync();
    }

    private bool IsExpanded(string participantId) => _expandedParticipants.Contains(participantId);

    private void SetExpanded(string participantId, bool expanded)
    {
        if (expanded)
        {
            _expandedParticipants.Add(participantId);
        }
        else
        {
            _expandedParticipants.Remove(participantId);
        }
    }

    private async Task ShowSelectLocationDialog()
    {
        var parameters = new DialogParameters<SelectLocationDialog> { { "CurrentUser", CurrentUser }, { "ShowAllOption", _selectedLocation is not null } };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectLocationDialog>("Select a location", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: LocationDto location })
        {
            _selectedLocation = location.Id == 0 ? null : location;
            _pageNumber = 1;
            await LoadDataAsync();
        }
    }

    private void EditParticipant(string participantId) => Navigation.NavigateTo($"/pages/workspace/participants/{participantId}?from=qa-enrolment-results");
}