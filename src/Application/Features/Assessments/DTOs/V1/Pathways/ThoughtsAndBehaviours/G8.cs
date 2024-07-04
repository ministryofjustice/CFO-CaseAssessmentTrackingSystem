namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class G8() : SingleChoiceQuestion("My emotions can get the better of me",
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
