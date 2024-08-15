using System.Text.Json.Serialization;

namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.RecentExperiences;

public sealed partial class RecentExperiencesPathway
    : PathwayBase
{

    [JsonIgnore]
    public override string Title => "RecentExperiences";



    [JsonIgnore]
    public override string Icon => CatsIcons.RecentExperiences;
    public override IEnumerable<QuestionBase> Questions()
    {
        yield return C1;
        yield return C2;
        yield return C3;
        yield return C4;
        yield return C5;
        yield return C6;
        yield return C7;
        yield return C8;
        yield return C9;
        yield return C10;

    }

    public C1 C1 { get; private set; } = new();
    public C2 C2 { get; private set; } = new();
    public C3 C3 { get; private set; } = new();
    public C4 C4 { get; private set; } = new();
    public C5 C5 { get; private set; } = new();
    public C6 C6 { get; private set; } = new();
    public C7 C7 { get; private set; } = new();
    public C8 C8 { get; private set; } = new();
    public C9 C9 { get; private set; } = new();
    public C10 C10 { get; private set; } = new();

}
