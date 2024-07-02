namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

public class A3 : SingleChoiceQuestion
{
    public override string Question => "Does or would your offence limit the types of work you could do?";
    public override double Weight => 0.09;

    public override string? OtherInformation => null;
    public override QuestionResponse[] Options =>
    [
        new("Yes", 1),
        new("No", 1),
        new("Not Sure", 1),
    ];
}
