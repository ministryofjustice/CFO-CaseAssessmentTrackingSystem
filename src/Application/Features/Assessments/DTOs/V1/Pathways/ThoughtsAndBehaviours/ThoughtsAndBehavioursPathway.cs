using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;
using System.Runtime.InteropServices;

namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.ThoughtsAndBehaviours;

public class ThoughtsAndBehavioursPathway
    : PathwayBase
{
    public ThoughtsAndBehavioursPathway()
    { 
        Questions =
        [
            G1 = new G1(),
            G2 = new H2(),
            G3 = new G3(),
            G4 = new G4(),
            G5 = new G5(),
            G6 = new G6(),
            G7 = new G7(),
            G8 = new G8(),
            G9 = new G9(),
            G10 = new G10(),
        ];
    }
    public G1 G1 { get; private set; }
    public H2 G2 { get; private set; }
    public G3 G3 { get; private set; }
    public G4 G4 { get; private set; }
    public G5 G5 { get; private set; }
    public G6 G6 { get; private set; }
    public G7 G7 { get; private set; }
    public G8 G8 { get; private set; }
    public G9 G9 { get; private set; }
    public G10 G10 { get; private set; }
    public override string Title => "Thoughts & Behaviours";
    public override double Constant => 0.68287;
    public override string Icon => CatsIcons.Thoughts;
    protected override IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex)
    {
        yield return 0;
    }
}