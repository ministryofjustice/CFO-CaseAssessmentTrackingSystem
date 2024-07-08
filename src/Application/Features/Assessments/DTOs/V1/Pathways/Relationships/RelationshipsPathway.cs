using System.Text.Json.Serialization;

namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public partial class RelationshipsPathway : PathwayBase
{
    public RelationshipsPathway()
    {
        F1 = new F1();
        F2 = new F2();
        F3 = new F3();
        F4 = new F4();
        F5 = new F5();
        F6 = new F6();
        F7 = new F7();
        F8 = new F8();
        F9 = new F9();
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

    [JsonIgnore]
    public override string Title => "Relationships";
    
    [JsonIgnore]
    public override double Constant => 0.68620;
    
    [JsonIgnore]
    public override string Icon => CatsIcons.Relationships;
    public override IEnumerable<QuestionBase> Questions()
    {
        yield return F1;
        yield return F2;
        yield return F3;
        yield return F4;
        yield return F5;
        yield return F6;
        yield return F7;
        yield return F8;
        yield return F9;
    }

}