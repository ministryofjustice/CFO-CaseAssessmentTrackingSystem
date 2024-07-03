namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.WellbeingAndMentalHealth;

public class WellbeingAndMentalHealthPathway
    : PathwayBase
{
    public override string Title => "Wellbeing & Mental Health";
    public override double Constant => 0.64723;
    public override string Icon => CatsIcons.Wellbeing;
    protected override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
    {
        yield return 0;
    }
}