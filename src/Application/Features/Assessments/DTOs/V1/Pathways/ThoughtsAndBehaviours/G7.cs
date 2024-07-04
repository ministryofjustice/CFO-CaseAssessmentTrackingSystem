namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class G7() : SingleChoiceQuestion("I struggle to understand my own or other people's feelings",
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
