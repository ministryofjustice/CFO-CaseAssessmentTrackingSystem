namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.ChildhoodExperiences;
public class B1() : SingleChoiceQuestion("Spent time in care as a child",
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