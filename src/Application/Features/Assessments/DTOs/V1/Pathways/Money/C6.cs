namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Money;

public class C6() : SingleChoiceQuestion("Do you have a valid ID?", "(e.g. passport)",
    [Yes, No])
{
    public const string Yes = "Yes";
    public const string No = "No";
}