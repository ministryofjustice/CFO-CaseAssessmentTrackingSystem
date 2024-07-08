using System.Text.Json.Serialization;

namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.ThoughtsAndBehaviours;

public partial class ThoughtsAndBehavioursPathway
    : PathwayBase
{
    public ThoughtsAndBehavioursPathway()
    {
        G1 = new G1();
        G2 = new G2();
        G3 = new G3();
        G4 = new G4();
        G5 = new G5();
        G6 = new G6();
        G7 = new G7();
        G8 = new G8();
        G9 = new G9();
        G10 = new G10();
    }
    public G1 G1 { get; private set; }
    public G2 G2 { get; private set; }
    public G3 G3 { get; private set; }
    public G4 G4 { get; private set; }
    public G5 G5 { get; private set; }
    public G6 G6 { get; private set; }
    public G7 G7 { get; private set; }
    public G8 G8 { get; private set; }
    public G9 G9 { get; private set; }
    public G10 G10 { get; private set; }
    
    [JsonIgnore]
    public override string Title => "Thoughts & Behaviours";
    
    [JsonIgnore]
    public override double Constant => 0.68287;
    
    [JsonIgnore]
    public override string Icon => CatsIcons.Thoughts;
    public override IEnumerable<QuestionBase> Questions()
    {
        yield return G1;
        yield return G2;
        yield return G3;
        yield return G4;
        yield return G5;
        yield return G6;
        yield return G7;
        yield return G8;
        yield return G9;
        yield return G10;
    }

}

