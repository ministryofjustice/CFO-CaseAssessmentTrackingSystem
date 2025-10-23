using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class MyArchivedCases
{
    private int _pageNumber = 1;
    private bool _canSearch;
    private string? _keyWord = null;

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = default!;

    protected override IRequest<Result<PaginatedData<ParticipantsArchivedResultsSummaryDto>>> CreateQuery()
        => new ParticipantsArchivedResultsWithPagination.Query()
        {
            PageSize = 5,
            OrderBy = "Created",
            SortDirection = $"{SortDirection.Descending}",
            CurrentUser=CurrentUser,            
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