using Cfo.Cats.Application.Common.Interfaces.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Cfo.Cats.Server.UI.Hubs;

/// <summary>
/// Tracks live connections to the site so the UI can show who is currently online and notify
/// users when someone comes online.
///
/// Every caller must be authenticated. Each connection is recorded against the user's display
/// name in <see cref="IUsersStateContainer"/>. To support tenant based visibility ("you can
/// only see someone at your level or below"), every connection also joins a SignalR group named
/// after its own tenant id. A user coming online is announced to all of their ancestor-or-equal
/// tenant scopes, so only users whose tenant is a prefix of the new user's tenant are notified.
/// Identity is always taken from <see cref="HubCallerContext.User"/> and never from values
/// supplied by a client.
/// </summary>
[Authorize(AuthenticationSchemes = "Identity.Application")]
public sealed class PresenceHub : Hub
{
    public const string HubUrl = "/hubs/presence";

    /// <summary>Client method invoked when a visible user comes online.</summary>
    public const string UserOnline = "UserOnline";

    private readonly IUsersStateContainer _usersStateContainer;

    public PresenceHub(IUsersStateContainer usersStateContainer)
    {
        _usersStateContainer = usersStateContainer;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.GetUserId();
        var name = Context.User?.GetDisplayName() ?? Context.User?.GetUserName();
        var tenantId = Context.User?.GetTenantId();

        // Whether this user already had a live connection (e.g. another tab) before this one.
        var alreadyOnline = !string.IsNullOrEmpty(name)
            && _usersStateContainer.UsersByConnectionId.Values
                .Any(v => v.Equals(name, StringComparison.OrdinalIgnoreCase));

        _usersStateContainer.AddOrUpdate(Context.ConnectionId, name);

        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, tenantId).ConfigureAwait(false);

            // Announce only the first time the user comes online, and only to viewers whose
            // tenant is the new user's tenant or an ancestor of it.
            if (!alreadyOnline && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(userId))
            {
                var scopes = AncestorTenantScopes(tenantId).ToList();
                await Clients.Groups(scopes).SendAsync(UserOnline, userId, name).ConfigureAwait(false);
            }
        }

        await base.OnConnectedAsync().ConfigureAwait(false);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _usersStateContainer.Remove(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
    }

    /// <summary>
    /// Yields the tenant id and each of its ancestors in dot notation, e.g. <c>1.2.3.</c> ->
    /// <c>1.</c>, <c>1.2.</c>, <c>1.2.3.</c>. These are the tenant scopes permitted to see a user
    /// at <paramref name="tenantId"/>.
    /// </summary>
    private static IEnumerable<string> AncestorTenantScopes(string tenantId)
    {
        var parts = tenantId.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var accumulated = string.Empty;
        foreach (var part in parts)
        {
            accumulated += part + ".";
            yield return accumulated;
        }
    }
}
