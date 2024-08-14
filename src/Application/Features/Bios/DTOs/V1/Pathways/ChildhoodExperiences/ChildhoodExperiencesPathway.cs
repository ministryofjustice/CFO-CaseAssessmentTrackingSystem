using System.Text.Json.Serialization;

namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.ChildhoodExperiences;

public sealed partial class ChildhoodExperiencesPathway 
    : PathwayBase
{

    [JsonIgnore]
    public override string Title => "ChildhoodExperiences";
    

    
    [JsonIgnore]
    public override string Icon => CatsIcons.ChildhoodExperiences;
    public override IEnumerable<QuestionBase> Questions()
    {
        yield return B1;
        yield return B2;
        yield return B3;
        yield return B4;
        yield return B5;
        yield return B6;
        yield return B7;
        yield return B8;
        yield return B9;
        yield return B10;
        yield return B11;
        yield return B12;
        yield return B13;
        yield return B14;
        yield return B15;

    }

    public B1 B1 { get; private set; } = new();
    public B2 B2 { get; private set; } = new();
    public B3 B3 { get; private set; } = new();
    public B4 B4 { get; private set; } = new();
    public B5 B5 { get;private set; } = new();
    public B6 B6 { get;private set; } = new();
    public B7 B7 { get;private set; } = new();
    public B8 B8 { get;private set; } = new();
    public B9 B9 { get; private set; } = new();
    public B10 B10 { get; private set; } = new();
    public B11 B11 { get; private set; } = new();
    public B12 B12 { get; private set; } = new();
    public B13 B13 { get; private set; } = new();
    public B14 B14 { get; private set; } = new();
    public B15 B15 { get; private set; } = new();
}
