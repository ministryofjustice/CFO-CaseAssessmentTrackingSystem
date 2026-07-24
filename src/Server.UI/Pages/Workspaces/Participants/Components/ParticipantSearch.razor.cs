using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Microsoft.AspNetCore.Components.Web;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Components;

public partial class ParticipantSearch
{
    private string? _searchTerm;
    
    protected override Task OnInitializedAsync()
    {
        Loading = false;
        return Task.CompletedTask;
    }
    
    protected override IQuery<Result<List<ParticipantSearchResultDto>>> CreateQuery()
        => new SearchParticipants.Query
        {
            Keyword = _searchTerm!,
            CurrentUser = CurrentUser,
            MaxResults = 100 
        };

    protected override void OnDataLoaded(List<ParticipantSearchResultDto> data)
    {
        if (data.Count == 1)
        {
            // Exact match - navigate directly to participant page
            Navigation.NavigateTo($"/pages/workspace/participants/{data[0].Id}");
        }
        else if (data.Count > 1)
        {
            // Multiple matches - navigate to all participants page with filter
            Navigation.NavigateTo($"/pages/workspace/participants/all?keyword={Uri.EscapeDataString(_searchTerm!)}");
        }
        else
        {
            // No results - this shouldn't happen as the query returns empty list, but handle it
            ErrorMessage = $"No participants found matching '{_searchTerm}'";
        }
    }

    private async Task OnKeyUp(KeyboardEventArgs args)
    {
        if (args.Key != "Enter")
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_searchTerm))
        {
            return;
        }

        _searchTerm = _searchTerm.Trim();
        ErrorMessage = null;

        await LoadDataAsync();
    }
}