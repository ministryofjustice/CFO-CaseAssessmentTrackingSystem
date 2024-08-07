using System.Text.Json.Serialization;

namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;

public sealed partial class HousingPathway : PathwayBase
{
    public HousingPathway()
    {
        B1 = new B1();
        B2 = new B2();
        B3 = new B3();
        B4 = new B4();
        B5 = new B5();
        B6 = new B6();
    }

    [JsonIgnore]
    public override string Title => "Housing";
    
    [JsonIgnore]
    public override double Constant => 0.87634D;
    
    [JsonIgnore]
    public override string Icon => CatsIcons.Housing;
    public override IEnumerable<QuestionBase> Questions()
    {
        yield return B1;
        yield return B2;
        yield return B3;
        yield return B4;
        yield return B5;
        yield return B6;
    }
    public B1 B1 { get; private set; }
    public B2 B2 { get; private set; }
    public B3 B3 { get; private set; }
    public B4 B4 { get; private set; }
    public B5 B5 { get; private set; }
    public B6 B6 { get; private set; }
    
    

}