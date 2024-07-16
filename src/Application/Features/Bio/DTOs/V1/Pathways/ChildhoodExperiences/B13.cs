namespace Cfo.Cats.Application.Features.Bio.DTOs.V1.Pathways.ChildhoodExperiences;
public class B13() : SingleChoiceQuestion("Parent or guardian was seriously ill / passed away",
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