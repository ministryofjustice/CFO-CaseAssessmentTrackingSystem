using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Server.UI.Components.Identity;
using Cfo.Cats.Server.UI.Components.Locations;
using Cfo.Cats.Server.UI.Pages.Participants.Components;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;

public class ParticipantDialogService : IParticipantDialogService
{
    private readonly IDialogService _dialogService;

    public ParticipantDialogService(IDialogService dialogService) => _dialogService = dialogService;

    public async Task<LocationDto?> PromptForLocationAsync(UserProfile currentUser)
    {
        var parameters = new DialogParameters<SelectLocationDialog>
        {
            { "CurrentUser", currentUser }
        };

        var options = new DialogOptions 
        { 
            CloseButton = true, 
            MaxWidth = MaxWidth.Large, 
            FullWidth = false,
            BackdropClick = false,
            CloseOnEscapeKey = true
        };

        var dialog = await _dialogService.ShowAsync<SelectLocationDialog>(
            "Select a location", 
            parameters, 
            options);

        var result = await dialog.Result;

        return result is { Canceled: false, Data: LocationDto location } 
            ? location 
            : null;
    }

    public async Task<SelectedUser?> PromptForAssigneeAsync(UserProfile currentUser)
    {
        var parameters = new DialogParameters<SelectUserDialog>
        {
            { "CurrentUser", currentUser }
        };

        var options = new DialogOptions 
        { 
            CloseButton = true, 
            MaxWidth = MaxWidth.Large, 
            FullWidth = false,
            BackdropClick = false,
            CloseOnEscapeKey = true
        };

        var dialog = await _dialogService.ShowAsync<SelectUserDialog>(
            "Select an assignee", 
            parameters, 
            options);

        var result = await dialog.Result;

        return result is { Canceled: false, Data: SelectedUser user } 
            ? user 
            : null;
    }

    public async Task<SelectedTenant?> PromptForTenantAsync(UserProfile currentUser)
    {
        var parameters = new DialogParameters<SelectTenantDialog>
        {
            { "CurrentUser", currentUser }
        };

        var options = new DialogOptions 
        { 
            CloseButton = true, 
            MaxWidth = MaxWidth.Large, 
            FullWidth = false,
            BackdropClick = false,
            CloseOnEscapeKey = true
        };

        var dialog = await _dialogService.ShowAsync<SelectTenantDialog>(
            "Select a tenant", 
            parameters, 
            options);

        var result = await dialog.Result;

        return result is { Canceled: false, Data: SelectedTenant tenant } 
            ? tenant 
            : null;
    }

    public async Task<bool> PromptForReassignAsync(UserProfile currentUser, string[] participantIds)
    {
        var parameters = new DialogParameters<ReassignParticipantDialog>
        {
            {
                x => x.Model, new ReassignParticipants.Command
                {
                    CurrentUser = currentUser,
                    ParticipantIdsToReassign = participantIds
                }
            },
            {
                x => x.UserProfile,
                currentUser
            }
        };

        var options = new DialogOptions 
        { 
            CloseButton = true, 
            MaxWidth = MaxWidth.Medium, 
            FullWidth = true,
            BackdropClick = false,
            CloseOnEscapeKey = true
        };

        var dialog = await _dialogService.ShowAsync<ReassignParticipantDialog>(
            "Reassign participants", 
            parameters, 
            options);

        var result = await dialog.Result;

        return result?.Canceled == false;
    }
}
