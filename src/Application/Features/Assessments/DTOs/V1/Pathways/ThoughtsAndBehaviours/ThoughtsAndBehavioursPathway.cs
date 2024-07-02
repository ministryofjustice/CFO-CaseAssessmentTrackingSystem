namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.ThoughtsAndBehaviours;

public class ThoughtsAndBehavioursPathway
    : PathwayBase
{
    public override string Title => "Thoughts & Behaviours";
    public override double Constant => 0.68287;
    public override string Icon => CatsIcons.Thoughts;
    protected override IEnumerable<double> GetScores()
    {
        yield return 0;
    }
}