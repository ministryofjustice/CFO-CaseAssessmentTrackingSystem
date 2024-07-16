namespace Cfo.Cats.Application.Features.Bio.DTOs.V1.Pathways.Diversity;

public class A10() : SingleChoiceQuestion("Are/have been a member of a gang",
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