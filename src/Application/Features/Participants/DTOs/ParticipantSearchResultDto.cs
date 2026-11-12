namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ParticipantSearchResultDto
{
    public required string Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? CurrentLocation { get; set; }
    public EnrolmentStatus? EnrolmentStatus { get; set; }
    public bool IsUnassigned { get; set; }

    /// <summary>
    /// Whether the current user can open the participant record. When false only
    /// the summary fields on this DTO should be shown.
    /// </summary>
    public bool HasAccess { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
