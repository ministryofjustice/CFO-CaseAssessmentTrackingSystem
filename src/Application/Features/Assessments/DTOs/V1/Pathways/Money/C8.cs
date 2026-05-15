namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Money;

public class C8()
 : SingleChoiceQuestion("Do you need help or support with benefits?", [
        Yes,
        No
 ])
{
    public override string Code => nameof(C8);
    public const string Yes = "Yes";
    public const string No = "No";
}