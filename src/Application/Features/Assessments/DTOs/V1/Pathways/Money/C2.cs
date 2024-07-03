namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Money;

public class C2()
    : SingleChoiceQuestion("Are you behind on credit card or store card payments or in paying your household bills?",
        [Yes, No])
{
    public const string Yes = "Yes";
    public const string No = "No";
}