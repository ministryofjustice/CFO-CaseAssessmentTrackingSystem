using Cfo.Cats.Application.Common.Security;
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
    /// <returns>The selected location, or null if canceled.</returns>
    Task<LocationDto?> PromptForLocationAsync(UserProfile currentUser);

    /// <summary>
    /// Shows the assignee (user) selection dialog.
    /// </summary>
    /// <param name="currentUser">The current user profile for authorization.</param>
    /// <returns>The selected user, or null if canceled.</returns>
    Task<SelectedUser?> PromptForAssigneeAsync(UserProfile currentUser);

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
