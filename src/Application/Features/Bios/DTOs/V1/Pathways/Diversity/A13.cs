namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.Diversity;

public class A13() : SingleChoiceQuestion("Feel you are being taken advantage of by others",
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