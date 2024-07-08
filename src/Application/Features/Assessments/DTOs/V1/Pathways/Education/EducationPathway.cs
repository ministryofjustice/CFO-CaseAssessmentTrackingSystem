using System.Text.Json.Serialization;

namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Education;

public partial class EducationPathway : PathwayBase
{
    public EducationPathway()
    {
        D1 = new D1();
        D2 = new D2();
        D3 = new D3();
        D4 = new D4();
        D5 = new D5();
    }
    public D1 D1 { get; private set; }
    public D2 D2 { get; private set; }

    public D3 D3 { get; private set; }

    public D4 D4 { get; private set; }

    public D5 D5 { get; private set; }

    [JsonIgnore]
    public override string Title => "Education";
    
    [JsonIgnore]
    public override double Constant => 0.61259;
    
    [JsonIgnore]
    public override string Icon => CatsIcons.Education;
    public override IEnumerable<QuestionBase> Questions()
    {
        yield return D1;
        yield return D2;
        yield return D3;
        yield return D4;
        yield return D5;
    }

}