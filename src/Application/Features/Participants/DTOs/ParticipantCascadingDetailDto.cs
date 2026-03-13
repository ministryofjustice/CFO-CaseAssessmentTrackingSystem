namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ParticipantCascadingDetailDto
{
    public required string Id { get; set; }
    public required string FullName { get; set; }
    public required bool IsActive { get; set; }
    public ConsentStatus? ConsentStatus { get; set; }
    public DateOnly? DateOfFirstConsent { get; set; }
}