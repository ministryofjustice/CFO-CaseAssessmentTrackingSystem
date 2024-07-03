namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class F3() : SingleChoiceQuestion("Are you interested in having a mentor?", "For example, someone to give you support, advice, and guidance to help you.",
    [
        Yes, 
        No, 
        NotSure
    ])
{
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NotSure = "Not sure";
}