namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public class Assessment
{
    public required string ParticipantId { get; set; }
    public required PathwayBase[] Pathways { get; set; }
}