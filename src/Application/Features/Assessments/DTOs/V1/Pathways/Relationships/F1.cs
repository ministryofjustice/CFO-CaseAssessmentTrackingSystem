namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class F1() : SingleChoiceQuestion("How happy are you with your current personal relationships?",
    [
        ExtremelyUnhappy,
        FairlyUnhappy,
        Happy,
        VeryHappy,
        Extremelyhappy,
    ])
{
    public const string ExtremelyUnhappy = "Extremely unhappy";
    public const string FairlyUnhappy = "Fairly unhappy";
    public const string Happy = "Happy";
    public const string VeryHappy = "Very happy";
    public const string Extremelyhappy = "Extremely happy";
}
