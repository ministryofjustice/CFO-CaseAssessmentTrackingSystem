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
    /// The groups on the current group-page, each carrying a limited set of participant rows.
    /// </summary>
    public ParticipantGroupDto[] Groups { get; set; } = [];
}

/// <summary>
/// A single group (e.g. one assignee) with its true total participant count and the subset of
/// participant rows currently loaded for it (expanded incrementally via "show more").
/// </summary>
public class ParticipantGroupDto
{
    /// <summary>
    /// The stable key used to fetch this group's participants (the owner id, or tenant id).
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// The human friendly label shown in the group header (assignee display name, or tenant name).
    /// </summary>
    public required string Label { get; set; }

    /// <summary>
    /// The true total number of participants in this group across all matching participants.
    /// </summary>
    public required int TotalCount { get; set; }

    /// <summary>
    /// The participant rows currently loaded for this group.
    /// </summary>
    public ParticipantPaginationDto[] Items { get; set; } = [];
}
