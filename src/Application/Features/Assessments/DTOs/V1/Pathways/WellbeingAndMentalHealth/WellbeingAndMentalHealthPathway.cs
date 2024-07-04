using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.WellbeingAndMentalHealth;

public class WellbeingAndMentalHealthPathway : PathwayBase
{
    public WellbeingAndMentalHealthPathway()
    {
        Questions =
        [
            H1 = new H1(),
            H2 = new H2(),
            H3 = new H3(),
            H4 = new H4(),
            H5 = new H5(),
            H6 = new H6(),
            H7 = new H7(),
            H8 = new H8(),
            H9 = new H9(),
            H10 = new H10(),
            H11 = new H11(),
        ];
    }
    public H1 H1 { get; private set; }
    public H2 H2 { get; private set; }
    public H3 H3 { get; private set; }
    public H4 H4 { get; private set; }
    public H5 H5 { get; private set; }
    public H6 H6 { get; private set; }
    public H7 H7 { get; private set; }
    public H8 H8 { get; private set; }
    public H9 H9 { get; private set; }
    public H10 H10 { get; private set; }
    public H11 H11 { get; private set; }

    public override string Title => "Wellbeing & Mental Health";
    public override double Constant => 0.64723;
    public override string Icon => CatsIcons.Wellbeing;
    protected override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
    {
        yield return 0;
    }
}