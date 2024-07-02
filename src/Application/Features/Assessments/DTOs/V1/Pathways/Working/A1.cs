namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

public class A1 : SingleChoiceQuestion
{
    public override string Question => "What is your current employment status?";
    public override double Weight => 0.5D;
    
    public override string? OtherInformation => "If you’re in prison, then think about what you expect to be doing after release.";
    public override QuestionResponse[] Options =>
    [
        new ("Do not want a job", 1D),
        new ("Want a job but cannot work", 0.29D),
        new ("Looking for work", 0.25D),
        new ("In a temporary job", 0.21D),
        new ("In a permanent job", 0.17D)
    ];

}