using System.Text.Json.Serialization;

namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

public sealed partial class WorkingPathway
    : PathwayBase
{

    [JsonIgnore]
    public override string Title => "Working";
    
    [JsonIgnore]
    public override double Constant => 0.79225;
    
    [JsonIgnore]
    public override string Icon => CatsIcons.Working;
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
    }
    /// <summary>
    ///     A1 What is your current employment status?
    /// </summary>
    public A1 A1 { get; private set; } = new();
    /// <summary>
    ///     A2 When were you last in work?
    /// </summary>
    public A2 A2 { get; private set; } = new();

    /// <summary>
    ///     A3 Does or would your offence limit the types of work you could do?
    /// </summary>
    public A3 A3 { get; private set; } = new();
    public A4 A4 { get; private set; } = new();
    public A5 A5 { get;private set; } = new();
    public A6 A6 { get;private set; } = new();
    public A7 A7 { get;private set; } = new();
    public A8 A8 { get;private set; } = new();
    public A9 A9 { get; private set; } = new();
    public A10 A10 { get;private set; } = new();
}
