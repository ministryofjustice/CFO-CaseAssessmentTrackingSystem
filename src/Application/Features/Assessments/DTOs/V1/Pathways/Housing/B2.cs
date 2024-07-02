namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;

public class B2 : MultipleChoiceQuestion
{

    public override string Question => "Are you facing any of the following problems? (tick all that apply)";
    public override double Weight => 0.02;
    public override string? OtherInformation => null;
    public override QuestionResponse[] Options =>
    [
        //TODO: check the scores here with Adam
        new("Behind on rent/mortgage", 0.09),
        new("Facing eviction", 1),
        new("Having to move due to licence restrictions", 1),
        new("Having to move due to domestic issues", 1),
        new("Worried may become homeless", 1),
        new("None of the above", 1),
    ];
    
    
    public override double Score(int participantAge, AssessmentLocation location, Sex sex)
    {
        var percentile = Options.Where(o => Answers!.Any(a => a == o.Answer))
            .Min(o => o.Percentile);
        
        var result = Math.Pow(percentile, Weight);
        return Math.Round(result, 5);
    }
}
