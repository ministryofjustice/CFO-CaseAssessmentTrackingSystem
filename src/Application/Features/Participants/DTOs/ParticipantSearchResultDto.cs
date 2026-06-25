namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ParticipantSearchResultDto
{
    public required string Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? CurrentLocation { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
