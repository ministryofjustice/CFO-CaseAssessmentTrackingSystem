namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ParticipantAddressDto
{
    public required string Address { get; set; }
    public required string UPRN { get; set; }
    public required double X_COORDINATE { get; set; }
    public required double Y_COORDINATE { get; set; }
}
