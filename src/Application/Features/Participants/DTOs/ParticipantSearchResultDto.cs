namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ParticipantSearchResultDto
{
    public required string Id { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public required string LastName { get; set; }
    public string? CurrentLocation { get; set; }
    public EnrolmentStatus? EnrolmentStatus { get; set; }
    public bool IsUnassigned { get; set; }

    public string FullName => string.Join(' ', new[] { FirstName, MiddleName, LastName }.Where(s => !string.IsNullOrWhiteSpace(s)));
}
