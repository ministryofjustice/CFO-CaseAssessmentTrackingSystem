using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Common.Interfaces;

/// <summary>
/// Captures local usage telemetry for "areas of concern" (which user, when, which
/// function). Implementations publish the event to the message bus for a background
/// consumer to persist. Published directly (not via the outbox) because the odd lost
/// event is acceptable and we never want to replay it.
/// </summary>
public interface IUsageTracker
{
    /// <summary>
    /// Record that a user exercised a given area/function.
    /// </summary>
    /// <param name="area">High level area of concern, e.g. "Participant", "Risk".</param>
    /// <param name="activity">The specific function performed, e.g. "ViewRiskScreen".</param>
    /// <param name="userProfile">The user performing the action.</param>
    /// <param name="context">Optional free-text context, e.g. a participant id.</param>
    Task TrackAsync(string area, string activity, UserProfile userProfile, string? context = null, CancellationToken cancellationToken = default);
}
