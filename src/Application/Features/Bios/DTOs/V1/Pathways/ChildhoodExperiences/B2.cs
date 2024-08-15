namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.ChildhoodExperiences;
public class B2() : SingleChoiceQuestion("Spent most of childhood in the same family home",
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