namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;

public sealed class HousingPathway : PathwayBase
{
    public HousingPathway()
    {
        Questions =
        [
            B2 = new B2()
        ];
    }

    public override string Title => "Housing";
    public override double Constant => 0.87634D;
    public override string Icon => CatsIcons.Housing;
    
    public B2 B2 { get; private set; }

   
}
