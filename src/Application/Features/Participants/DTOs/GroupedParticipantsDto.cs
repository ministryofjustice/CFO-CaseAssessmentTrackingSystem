namespace Cfo.Cats.Application.Features.Participants.DTOs;

/// <summary>
/// A single page of participant groups (e.g. assignees) together with the total number of groups
/// available, so the caller can paginate across the groups themselves.
/// </summary>
public class GroupedParticipantsDto
{
    /// <summary>
    /// The total number of distinct groups matching the filter (used to paginate the groups).
    /// </summary>
    public int TotalGroups { get; set; }

    /// <summary>
    /// The groups on the current group-page. Each carries its key and total count; the participant
    /// rows are fetched lazily when the group is expanded.
    /// </summary>
    public ParticipantGroupDto[] Groups { get; set; } = [];
}

/// <summary>
/// A single group header (e.g. one assignee) — its key, label and total participant count.
/// Participants are loaded on demand when the group is expanded.
/// </summary>
public class ParticipantGroupDto
{
    /// <summary>
    /// The grouping key (assignee owner id, or tenant id) used to fetch this group's participants.
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// The human friendly label shown in the group header (assignee display name, or tenant name).
    /// </summary>
    public required string Label { get; set; }

    /// <summary>
    /// The total number of participants belonging to this group.
    /// </summary>
    public int Count { get; set; }
}
