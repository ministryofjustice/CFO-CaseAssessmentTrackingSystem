namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

public class A2 : SingleChoiceQuestion
{

    public override string Question => "When were you last in work?";
    public override double Weight => 0.15D;

    public override string? OtherInformation => null;
    public override QuestionResponse[] Options =>
    [
        new("I have never worked", 1),
        new("Over a year ago", 0.22),
        new("In the last year", 0.25),
        new("I am currently working", 0.06)
    ];
}