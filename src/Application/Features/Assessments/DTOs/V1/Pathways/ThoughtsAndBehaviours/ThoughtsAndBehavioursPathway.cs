namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.ThoughtsAndBehaviours;

public class ThoughtsAndBehavioursPathway
    : PathwayBase
{
    public override string Title => "Thoughts & Behaviours";
    public override double Constant => 0.68287;
    public override string Icon => CatsIcons.Thoughts;
    protected override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
    {
        yield return 0;
    }
}