using System.Text.Json.Serialization;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.WellbeingAndMentalHealth;

public partial class WellbeingAndMentalHealthPathway : PathwayBase
{
    public WellbeingAndMentalHealthPathway()
    {
        H1 = new H1();
        H2 = new H2();
        H3 = new H3();
        H4 = new H4();
        H5 = new H5();
        H6 = new H6();
        H7 = new H7();
        H8 = new H8();
        H9 = new H9();
        H10 = new H10();
        H11 = new H11();
        H12 = new H12();
        H13 = new H13();
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
    public H12 H12 { get; private set; }
    public H13 H13 { get; private set; }

    [JsonIgnore]
    public override string Title => "Wellbeing & Mental Health";
    
    [JsonIgnore]
    public override double Constant => 0.64723;
    
    [JsonIgnore]
    public override string Icon => CatsIcons.Wellbeing;
    public override IEnumerable<QuestionBase> Questions()
    {
        yield return H1;
        yield return H2;
        yield return H3;
        yield return H4;
        yield return H5;
        yield return H6;
        yield return H7;
        yield return H8;
        yield return H9;
        yield return H10;
        yield return H11;
        yield return H12;
        yield return H13;
    }

}