namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public abstract class PathwayBase
{
    public abstract string Title { get; }

    public abstract double Constant { get; }

    public abstract string Icon { get; }
    public QuestionBase[] Questions { get; protected set; } = [];

    public double Score(int age, AssessmentLocation location, Sex sex)
    {
        var scores = GetScores(age, location, sex);
        var score = scores.Aggregate(1.0, (ac, a) => ac * a);
        return Math.Round(score, 5);
    }

    protected abstract IEnumerable<double> GetScores(int age, AssessmentLocation location, Sex sex);
}