using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Server.UI.Components.Identity;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;

/// <summary>
/// Service for showing participant-related dialogs with pre-configured options.
/// Simplifies dialog invocation by encapsulating MudBlazor dialog setup.
/// </summary>
public interface IParticipantDialogService
{
    /// <summary>
    /// Shows the location selection dialog.
    /// </summary>
    /// <param name="currentUser">The current user profile for authorization.</param>
    /// <param name="title">Optional dialog title. Defaults to "Select a location".</param>
    /// <returns>The selected location, or null if canceled.</returns>
    Task<LocationDto?> PromptForLocationAsync(UserProfile currentUser, Func<LocationDto, bool>? filter = null, string title = "Select a location");

    /// <summary>
    /// Shows the assignee (user) selection dialog.
    /// </summary>
    /// <param name="currentUser">The current user profile for authorization.</param>
    /// <param name="title">Optional dialog title. Defaults to "Select an assignee".</param>
    /// <returns>The selected user, or null if canceled.</returns>
    Task<SelectedUser?> PromptForAssigneeAsync(UserProfile currentUser, string title = "Select an assignee", Func<ApplicationUserDto, bool>? filter = null);

    /// <summary>
    /// Shows the tenant selection dialog.
    /// </summary>
    /// <param name="currentUser">The current user profile for authorization.</param>
    /// <returns>The selected tenant, or null if canceled.</returns>
    Task<SelectedTenant?> PromptForTenantAsync(UserProfile currentUser);

    /// <summary>
    /// Shows reassign participants dialog.
    /// </summary>
    /// <param name="currentUser">The current user profile.</param>
    /// <param name="participantIds">The participant IDs to reassign.</param>
    /// <returns>True if reassignment was successful, false if canceled.</returns>
    Task<bool> PromptForReassignAsync(UserProfile currentUser, string[] participantIds);
}
