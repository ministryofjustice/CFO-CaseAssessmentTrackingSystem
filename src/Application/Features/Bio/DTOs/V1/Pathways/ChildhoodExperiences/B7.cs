namespace Cfo.Cats.Application.Features.Bio.DTOs.V1.Pathways.ChildhoodExperiences;
public class B7() : SingleChoiceQuestion("A parent or guardian was in trouble with the police",
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