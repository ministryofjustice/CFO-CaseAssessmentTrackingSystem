namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;

public class E3() : SingleChoiceQuestion("Are you currently taking any regular medication or undergoing treatment for a physical or mental health condition?",
    [
        Yes,
        No,
        NotSure
    ])
{
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NotSure = "Not Sure";
}