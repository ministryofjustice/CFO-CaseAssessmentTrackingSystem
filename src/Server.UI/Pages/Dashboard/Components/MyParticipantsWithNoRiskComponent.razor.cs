using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class MyParticipantsWithNoRiskComponent
{
    private int _pageNumber = 1;
    private bool _canSearch;
    private string? _keyWord = null;

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = default!;

    protected override IRequest<Result<PaginatedData<ParticipantsWithNoRiskDto>>> CreateQuery()
        => new ParticipantsWithNoRiskResultsWithPagination.Query()
        {
            PageSize = 5,
            OrderBy = "Created",
            SortDirection = $"{SortDirection.Descending}",
            CurrentUser = CurrentUser,
            JustMyParticipants = false,
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

    private string _searchString = "";
    
    private bool FilterFunc(ParticipantsWithNoRiskDto data) => FilterFunc(data, _searchString);

    private bool FilterFunc(ParticipantsWithNoRiskDto data, string searchString)
    {
        if (string.IsNullOrEmpty(searchString))
        {
            return true;
        }

        if (data.ParticipantId.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if (data.ParticipantName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        return false;
    } 
}