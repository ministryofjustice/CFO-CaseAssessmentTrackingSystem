namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.Diversity;

public class A8() : SingleChoiceQuestion("Undertook an operational tour with the British Armed Forces",
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