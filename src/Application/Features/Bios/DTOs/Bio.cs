namespace Cfo.Cats.Application.Features.Bios.DTOs;

public class Bio
{
    public required Guid Id { get; set; }
    public required string ParticipantId { get; set; }
    public BioStatus Status { get; set; } = BioStatus.NotStarted;
    public required PathwayBase[] Pathways { get; set; }
}