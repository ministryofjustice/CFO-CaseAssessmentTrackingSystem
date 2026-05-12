namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class F1() : SingleChoiceQuestion("How happy are you with your current personal relationships?",
    [
        ExtremelyUnhappy,
        FairlyUnhappy,
        Happy,
        VeryHappy,
        ExtremelyHappy,
    ])
{
    public override string Code => nameof(F1);
    public const string ExtremelyUnhappy = "Extremely unhappy";
    public const string FairlyUnhappy = "Fairly unhappy";
    public const string Happy = "Happy";
    public const string VeryHappy = "Very happy";
    public const string ExtremelyHappy = "Extremely happy";
}
