namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.ThoughtsAndBehaviours;

public class G3() : SingleChoiceQuestion("I am really working hard to change my life",
    [
        StronglyDisagree,
        Disagree,
        Neither,
        Agree,
        StronglyAgree
    ])
{
    public override string Code => nameof(G3);
    public const string StronglyDisagree = "Strongly disagree";
    public const string Disagree = "Disagree";
    public const string Neither = "Neither";
    public const string Agree = "Agree";
    public const string StronglyAgree = "Strongly agree";
}
