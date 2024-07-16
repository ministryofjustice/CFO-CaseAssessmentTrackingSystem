using System.Text.Json.Serialization;

namespace Cfo.Cats.Application.Features.Bio.DTOs.V1.Pathways.Diversity;

public sealed partial class DiversityPathway 
    : PathwayBase
{

    [JsonIgnore]
    public override string Title => "Diversity";
    

    
    [JsonIgnore]
    public override string Icon => CatsIcons.Diversity;
    public override IEnumerable<QuestionBase> Questions()
    {
        yield return A1;
        yield return A2;
        yield return A3;
        yield return A4;
        yield return A5;
        yield return A6;
        yield return A7;
        yield return A8;
        yield return A9;
        yield return A10;
        yield return A11;
        yield return A12;
        yield return A13;
        yield return A14;
        yield return A15;
        yield return A16;
        yield return A17;
        yield return A18;
        yield return A19;
        yield return A20;
    }

    public A1 A1 { get; private set; } = new();
    public A2 A2 { get; private set; } = new();
    public A3 A3 { get; private set; } = new();
    public A4 A4 { get; private set; } = new();
    public A5 A5 { get;private set; } = new();
    public A6 A6 { get;private set; } = new();
    public A7 A7 { get;private set; } = new();
    public A8 A8 { get;private set; } = new();
    public A9 A9 { get; private set; } = new();
    public A10 A10 { get; private set; } = new();
    public A11 A11 { get; private set; } = new();
    public A12 A12 { get; private set; } = new();
    public A13 A13 { get; private set; } = new();
    public A14 A14 { get; private set; } = new();
    public A15 A15 { get; private set; } = new();
    public A16 A16 { get; private set; } = new();
    public A17 A17 { get; private set; } = new();
    public A18 A18 { get; private set; } = new();
    public A19 A19 { get; private set; } = new();
    public A20 A20 { get; private set; } = new();

}
