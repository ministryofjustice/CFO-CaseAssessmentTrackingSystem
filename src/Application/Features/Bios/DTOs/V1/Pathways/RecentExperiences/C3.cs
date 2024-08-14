namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.RecentExperiences;
public class C3() : SingleChoiceQuestion("Became a parent",
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