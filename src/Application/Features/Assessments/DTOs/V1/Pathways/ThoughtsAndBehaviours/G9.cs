namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class G9() : SingleChoiceQuestion("I find it easy to talk to other people",
    [
        StronglyDisagree,
        Disagree,
        Neither,
        Agree,
        StronglyAgree
    ])
{
    public const string StronglyDisagree = "Strongly disagree";
    public const string Disagree = "Disagree";
    public const string Neither = "Neither";
    public const string Agree = "Agree";
    public const string StronglyAgree = "Strongly agree";
}
