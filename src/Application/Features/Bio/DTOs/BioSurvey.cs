namespace Cfo.Cats.Application.Features.Bio.DTOs;

public class BioSurvey
{
    public required Guid Id { get; set; }
    public required string ParticipantId { get; set; }
    public required PathwayBase[] Pathways { get; set; }
}