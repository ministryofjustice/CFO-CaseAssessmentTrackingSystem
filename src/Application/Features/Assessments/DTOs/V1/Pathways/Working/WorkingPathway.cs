namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

public sealed class WorkingPathway
    : PathwayBase
{

    public WorkingPathway()
    {
        Questions = [
                A1 = new A1(),
                A2 = new A2(),
                A3 = new A3()
            ];
    }

    public override string Title => "Wellbeing & Mental Health";
    public override double Constant => 0.79225D;
    public override string Icon => CatsIcons.Working;

    public A1 A1 { get; private set; }
    public A2 A2 { get; private set; }
    public A3 A3 { get; private set; }

    


} 
    

