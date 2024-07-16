namespace Cfo.Cats.Application.Features.Bio.DTOs.V1.Pathways.ChildhoodExperiences;
public class B10() : SingleChoiceQuestion("Felt safe at home",
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