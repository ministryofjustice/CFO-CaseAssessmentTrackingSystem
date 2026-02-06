using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Microsoft.AspNetCore.Components.Web;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class ParticipantById
{
    private string? _participantId;
    
    protected override Task OnInitializedAsync()
    {
        Loading = false;
        return Task.CompletedTask;
    }
    
    protected override IRequest<Result<ParticipantDto>> CreateQuery()
        => new GetParticipantByIdResult.Query
        {
            ParticipantId = _participantId!
        };

    protected override void OnDataLoaded(ParticipantDto data) => Navigation.NavigateTo($"/pages/participants/{data.Id}");

    private async Task OnKeyUp(KeyboardEventArgs args)
    {
        if (args.Key != "Enter")
        {
            return;
        }

        _participantId = _participantId!.Trim().ToUpperInvariant();
        ErrorMessage = null;

        await LoadDataAsync();
    }
}