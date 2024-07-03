namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class F4() : SingleChoiceQuestion("Do you feel you could be a suitable mentor for someone else?",
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