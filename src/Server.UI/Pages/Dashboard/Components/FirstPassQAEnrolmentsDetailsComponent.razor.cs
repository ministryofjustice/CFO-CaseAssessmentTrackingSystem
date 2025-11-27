using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class FirstPassQAEnrolmentsDetailsComponent
{
    [EditorRequired, Parameter]
    public DateRange? DateRange { get; set; }
    
    private int _pageNumber = 1;
    private bool _canSearch;
    private string? _keyWord;

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = default!;

    protected override IRequest<Result<PaginatedData<FirstPassQADetailsDto>>> CreateQuery()
        => new FirstPassQAEnrolmentsResultsWithPagination.Query()
        {
            CurrentUser = CurrentUser,
            StartDate = DateRange?.Start ?? throw new InvalidOperationException("DateRange not set"),
            EndDate = DateRange?.End ?? throw new InvalidOperationException("DateRange not set"),
            PageSize = 5,
            OrderBy = "Created",
            SortDirection = $"{SortDirection.Descending}",
            JustMyParticipants = true,
            PageNumber = _pageNumber,
            Keyword = _keyWord
        };

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;

        _canSearch = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.CandidateSearch)).Succeeded;

        await LoadDataAsync();
    }

    private Task OnPaginationChanged(int arg)
    {
        _pageNumber = arg;
        return LoadDataAsync();
    }

    private Task OnSearch(string text)
    {
        _keyWord = text;
        return LoadDataAsync();
    }

    private void EditParticipant(string participantId) => Navigation.NavigateTo($"/pages/participants/{participantId}");
}