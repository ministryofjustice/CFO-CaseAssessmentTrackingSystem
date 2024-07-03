namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Money;

public class C5()
    : SingleChoiceQuestion("Do you have a bank account?", [Yes, No])
{
    public const string Yes = "Yes";
    public const string No = "No";
}