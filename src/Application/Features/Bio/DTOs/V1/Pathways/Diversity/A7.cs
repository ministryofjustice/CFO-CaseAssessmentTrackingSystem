namespace Cfo.Cats.Application.Features.Bio.DTOs.V1.Pathways.Diversity;

public class A7() : SingleChoiceQuestion("Served in the British Armed Forces, inc. Reserves",
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