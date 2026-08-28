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
    /// <param name="title">Optional dialog title. Defaults to "Select a location".</param>
    /// <param name="showAllOption">When true, offers an "All" option at the top of the list so an existing filter can be cleared. Should only be set when a location filter is currently applied.</param>
    /// <param name="allOptionLabel">The label shown for the "All" option.</param>
    /// <returns>The selected location, or null if canceled.</returns>
    Task<LocationDto?> PromptForLocationAsync(UserProfile currentUser, Func<LocationDto, bool>? filter = null, string title = "Select a location", bool showAllOption = false, string allOptionLabel = "All Locations");

    /// <summary>
    /// Shows the assignee (user) selection dialog.
    /// </summary>
    /// <param name="currentUser">The current user profile for authorization.</param>
    /// <param name="title">Optional dialog title. Defaults to "Select an assignee".</param>
    /// <param name="showAllOption">When true, offers an "All" option at the top of the list so an existing filter can be cleared. Should only be set when an assignee filter is currently applied.</param>
    /// <param name="allOptionLabel">The label shown for the "All" option.</param>
    /// <returns>The selected user, or null if canceled.</returns>
    Task<SelectedUser?> PromptForAssigneeAsync(UserProfile currentUser, string title = "Select an assignee", bool showAllOption = false, string allOptionLabel = "All Users");

    /// <summary>
    /// Shows the tenant selection dialog.
    /// </summary>
    /// <param name="currentUser">The current user profile for authorization.</param>
    /// <param name="showAllOption">When true, offers an "All" option at the top of the list so an existing filter can be cleared. Should only be set when a tenant filter is currently applied.</param>
    /// <param name="allOptionLabel">The label shown for the "All" option.</param>
    /// <returns>The selected tenant, or null if canceled.</returns>
    Task<SelectedTenant?> PromptForTenantAsync(UserProfile currentUser, bool showAllOption = false, string allOptionLabel = "All Tenants");

    /// <summary>
    /// Shows reassign participants dialog.
    /// </summary>
    /// <param name="currentUser">The current user profile.</param>
    /// <param name="participantIds">The participant IDs to reassign.</param>
    /// <returns>True if reassignment was successful, false if canceled.</returns>
    Task<bool> PromptForReassignAsync(UserProfile currentUser, string[] participantIds);
}
