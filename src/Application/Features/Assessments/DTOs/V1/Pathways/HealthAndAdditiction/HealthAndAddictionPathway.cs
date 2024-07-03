namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;

public class HealthAndAddictionPathway : PathwayBase
{
    public override string Title => "Health & Addiction";
    public override double Constant => 0.76469;
    public override string Icon => CatsIcons.Health;
    protected override IEnumerable<double> GetScores(int age, AssessmentLocation location, Sex sex)
    {
        yield return 0;
    }
}