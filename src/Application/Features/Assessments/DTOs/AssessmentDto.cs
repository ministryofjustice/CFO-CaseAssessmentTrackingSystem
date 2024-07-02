namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public class AssessmentDto
{
    public List<AssessmentPathwayDto> Pathways { get; set; } = new();

    public void AddPathway(AssessmentPathwayDto pathway)
    {
        Pathways.Add(pathway);
    }
}