using Cfo.Cats.Domain.Common.Entities;

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

/// <summary>
/// A lightweight, local telemetry record capturing that a user exercised a
/// particular area/function of the system. Deliberately generic so that any
/// "area of concern" can be tracked without introducing an external analytics
/// provider (kept local for GDPR reasons).
/// </summary>
public class UsageEvent : BaseEntity<Guid>
{
    private UsageEvent() { }

    public static UsageEvent Create(
        string area,
        string activity,
        string? userId,
        string? userName,
        string? tenantId,
        string? context,
        DateTime occurredOn) => new()
    {
        Id = Guid.CreateVersion7(),
        Area = area,
        Activity = activity,
        UserId = userId,
        UserName = userName,
        TenantId = tenantId,
        Context = context,
        OccurredOn = occurredOn
    };

    /// <summary>The high level area of concern, e.g. "Participant", "Risk".</summary>
    public string Area { get; set; }

    /// <summary>The specific function performed, e.g. "ViewRiskScreen".</summary>
    public string Activity { get; set; }

    /// <summary>The id of the user who performed the action, if known.</summary>
    public string? UserId { get; set; }

    /// <summary>The username (typically email) of the user, if known.</summary>
    public string? UserName { get; set; }

    /// <summary>The tenant the user belonged to, if known.</summary>
    public string? TenantId { get; set; }

    /// <summary>Optional free-text context, e.g. a participant id or identifier.</summary>
    public string? Context { get; set; }

    /// <summary>When the tracked action occurred (UTC).</summary>
    public DateTime OccurredOn { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
