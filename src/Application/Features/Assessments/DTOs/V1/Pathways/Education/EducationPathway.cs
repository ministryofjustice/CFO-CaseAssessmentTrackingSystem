namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Education;

public class EducationPathway
    : PathwayBase
{
    public override string Title => "Education";
    public override double Constant => 0.61259;
    public override string Icon => CatsIcons.Education;
    protected override IEnumerable<double> GetScores(int age, AssessmentLocation location, Sex sex)
    {
        yield return 0;
    }
}