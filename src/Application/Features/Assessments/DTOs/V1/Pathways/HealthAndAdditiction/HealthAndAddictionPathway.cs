using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Education;

namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;

public class HealthAndAddictionPathway : PathwayBase
{
    public HealthAndAddictionPathway()
    {
        Questions =
        [
            E1 = new E1(),
            E2 = new E2(),
            E3 = new E3(),
            E4 = new E4(),
            E5 = new E5(),
            E6 = new E6(),
            E7 = new E7(),
            E8 = new E8(),
            E9 = new E9(),
        ];
    }
    public E1 E1 { get; private set; }
    public E2 E2 { get; private set; }
    public E3 E3 { get; private set; }
    public E4 E4 { get; private set; }
    public E5 E5 { get; private set; }
    public E6 E6 { get; private set; }
    public E7 E7 { get; private set; }
    public E8 E8 { get; private set; }
    public E9 E9 { get; private set; }

    public override string Title => "Health & Addiction";
    public override double Constant => 0.76469;
    public override string Icon => CatsIcons.Health;
    protected override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
    {
        yield return 0;
    }
}