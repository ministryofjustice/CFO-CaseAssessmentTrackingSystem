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
    /// The groups on the current group-page, each carrying its participant rows.
    /// </summary>
    public ParticipantGroupDto[] Groups { get; set; } = [];
}

/// <summary>
/// A single group (e.g. one assignee) with the participant rows that belong to it.
/// </summary>
public class ParticipantGroupDto
{
    /// <summary>
    /// The human friendly label shown in the group header (assignee display name, or tenant name).
    /// </summary>
    public required string Label { get; set; }

    /// <summary>
    /// The participants belonging to this group.
    /// </summary>
    public ParticipantPaginationDto[] Items { get; set; } = [];
}
