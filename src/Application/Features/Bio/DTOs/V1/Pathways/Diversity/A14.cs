namespace Cfo.Cats.Application.Features.Bio.DTOs.V1.Pathways.Diversity;

public class A14() : SingleChoiceQuestion("Feel you are part of a community ",
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