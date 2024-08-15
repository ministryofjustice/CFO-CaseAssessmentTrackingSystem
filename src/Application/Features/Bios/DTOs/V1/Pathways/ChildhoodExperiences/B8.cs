namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.ChildhoodExperiences;
public class B8() : SingleChoiceQuestion("A parent or guardian spent time in prison",
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