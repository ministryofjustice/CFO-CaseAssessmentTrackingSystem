namespace Cfo.Cats.Application.Features.Bio.DTOs.V1.Pathways.ChildhoodExperiences;
public class B6() : SingleChoiceQuestion("At least one parent or guardian was mostly in work",
[
    Yes,
    No,
    NA
])
{
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "N/A";
};