using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Server.UI.Components.Identity;
using Cfo.Cats.Server.UI.Components.Locations;
using Cfo.Cats.Server.UI.Pages.Participants.Components;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;

public class ParticipantDialogService(IDialogService dialogService) : IParticipantDialogService
{
    public async Task<LocationDto?> PromptForLocationAsync(UserProfile currentUser, Func<LocationDto, bool>? filter = null, string title = "Select a location")
    {
        var parameters = new DialogParameters<SelectLocationDialog>
        {
            { "CurrentUser", currentUser },
            { "Filter", filter }
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = false,
            BackdropClick = false,
            CloseOnEscapeKey = true
        };

        var dialog = await dialogService.ShowAsync<SelectLocationDialog>(
            title,
            parameters,
            options);

        var result = await dialog.Result;

        return result is { Canceled: false, Data: LocationDto location }
            ? location
            : null;
    }

    public async Task<SelectedUser?> PromptForAssigneeAsync(UserProfile currentUser, string title = "Select an assignee")
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

        var dialog = await dialogService.ShowAsync<SelectUserDialog>(
            title,
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

        var dialog = await dialogService.ShowAsync<SelectTenantDialog>(
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

        var dialog = await dialogService.ShowAsync<ReassignParticipantDialog>(
            "Reassign participants",
            parameters,
            options);

        var result = await dialog.Result;

        return result?.Canceled == false;
    }
}
