namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Money;

public class C1() : SingleChoiceQuestion("How are you coping with money?", 
    "If you’re in prison, then think about the time before you came into prison.",
    [
        FindingVeryDifficult,
        FindingQuiteDifficult,
        JustAboutGettingBy,
        DoingAlright,
        LivingComfortably
    ])
{
    public const string FindingVeryDifficult = "Finding it very difficult";
    public const string FindingQuiteDifficult = "Finding it quite difficult";
    public const string JustAboutGettingBy= "Just about getting by";
    public const string DoingAlright = "Doing alright";
    public const string LivingComfortably = "Living comfortably";
}