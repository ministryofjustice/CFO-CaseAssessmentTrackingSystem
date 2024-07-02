namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public abstract class PathwayBase
{
    public abstract string Title { get; }
    
    public abstract double Constant { get; }
   
    public abstract string Icon { get; }
    public QuestionBase[] Questions { get; protected set; } = [];

    public double Score(int participantAge, AssessmentLocation location, Sex sex)
    {
        var score = Questions
            .Select(q => q.Score(participantAge, location, sex))
            .Aggregate(1.0, (acc, val) => acc * val);

        return Math.Round(score, 5);
    }

}

public class Assessment
{
    public required PathwayBase[] Pathways { get; set; }
}