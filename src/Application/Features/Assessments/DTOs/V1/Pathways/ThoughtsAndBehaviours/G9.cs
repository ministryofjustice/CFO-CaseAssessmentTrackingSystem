namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.ThoughtsAndBehaviours;

public class G9() : SingleChoiceQuestion("I find it easy to talk to other people",
    [
        StronglyDisagree,
        Disagree,
        Neither,
        Agree,
        StronglyAgree
    ])
{
    public override string Code => nameof(G9);
    public const string StronglyDisagree = "Strongly disagree";
    public const string Disagree = "Disagree";
    public const string Neither = "Neither";
    public const string Agree = "Agree";
    public const string StronglyAgree = "Strongly agree";
}
