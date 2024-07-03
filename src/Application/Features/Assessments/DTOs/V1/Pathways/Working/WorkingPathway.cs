namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

public sealed partial class WorkingPathway
    : PathwayBase
{
    public WorkingPathway()
    {
        Questions =
        [
            A1 = new A1(),
            A2 = new A2(),
            A3 = new A3(),
            A4 = new A4(),
            A5 = new A5(),
            A6 = new A6(),
            A7 = new A7(),
            A8 = new A8(),
            A9 = new A9(),
            A10 = new A10()
        ];
    }

    public override string Title => "Wellbeing & Mental Health";
    public override double Constant => 0.79225;
    public override string Icon => CatsIcons.Working;
    public A1 A1 { get; private set; }
    public A2 A2 { get; private set; }
    public A3 A3 { get; private set; }
    public A4 A4 { get; private set; }
    public A5 A5 { get; private set; }
    public A6 A6 { get; private set; }
    public A7 A7 { get; private set; }
    public A8 A8 { get; private set; }
    public A9 A9 { get; private set; }
    public A10 A10 { get; private set; }
    
}




