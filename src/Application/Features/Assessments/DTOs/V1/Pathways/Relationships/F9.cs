namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class F9() : SingleChoiceQuestion("Which of the following best describes your parental situation?",
    "Try to select the option which best describes your situation.",
    [
        NoChildren,
        ChildrenNoCareResponsibility,
        AdultChildrenOrLeftHome,
        SharedCareResponsibility,
        LoneParent,
    ])
{
    public const string NoChildren = "I do not have children";
    public const string ChildrenNoCareResponsibility = "I have children but I am not responsible for their care";
    public const string AdultChildrenOrLeftHome = "My children are adults / have left home";
    public const string SharedCareResponsibility = "I share their care with someone else (e.g., your partner)";
    public const string LoneParent = "I consider myself a lone parent";
}