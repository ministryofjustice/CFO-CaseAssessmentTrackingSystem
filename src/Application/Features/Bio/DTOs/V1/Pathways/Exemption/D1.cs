namespace Cfo.Cats.Application.Features.Bio.DTOs.V1.Pathways.Exemption;
public class B1() : SingleChoiceQuestion("Exempted from bio survey?",
[
    Yes,
    No
])
{
    public const string Yes = "Yes";
    public const string No = "No";
};