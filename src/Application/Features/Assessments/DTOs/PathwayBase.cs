using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public abstract class PathwayBase
{
    [JsonIgnore]
    public abstract string Title { get; }

    [JsonIgnore]
    public abstract double Constant { get; }

    [JsonIgnore]
    public abstract string Icon { get; }

    public abstract IEnumerable<QuestionBase> Questions();
    

    public double GetPercentile(int age, AssessmentLocation location, Sex sex)
    {
        var scores = GetPercentiles(age, location, sex);
        var score = scores.Aggregate(1.0, (ac, a) => ac * a);
        return Math.Round(score, 5);
    }

    internal abstract IEnumerable<double> GetPercentiles(int age, AssessmentLocation location, Sex sex);

    public double GetRagScore(int age, AssessmentLocation location, Sex sex)
    {
        // see the CFO Evolution Assessment - RAG Scores document for 
        // an explanation of this math.
        var cumulativePercentiles = GetPercentile(age, location, sex);
        
        var a = (1 - cumulativePercentiles) * Constant;
        var b = (1 - Constant) * cumulativePercentiles;
        var c = a / b;
        var d = 1 / Constant;
        var e = Math.Pow(c, d);
        var f = 1 + e;
        return Math.Round(100 / f, 0);
    }
}