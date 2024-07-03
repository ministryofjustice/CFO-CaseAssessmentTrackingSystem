namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class RelationshipsPathway : PathwayBase
{
    public RelationshipsPathway()
    {
        Questions =
        [
            F1 = new F1(),
            F2 = new F2(),
            F3 = new F3(),
            F4 = new F4(),
            F5 = new F5(),
            F6 = new F6(),
            F7 = new F7(),
            F8 = new F8(),
            F9 = new F9(),
        ];
    }
    public F1 F1 { get; private set; }
    public F2 F2 { get; private set; }
    public F3 F3 { get; private set; }
    public F4 F4 { get; private set; }
    public F5 F5 { get; private set; }
    public F6 F6 { get; private set; }
    public F7 F7 { get; private set; }
    public F8 F8 { get; private set; }
    public F9 F9 { get; private set; }

    public override string Title => "Relationships";
    public override double Constant => 0.68620;
    public override string Icon => CatsIcons.Relationships;
    protected override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
    {
        yield return 0;
    }
}