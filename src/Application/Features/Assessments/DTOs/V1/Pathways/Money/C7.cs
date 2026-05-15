namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Money;

public class C7() : SingleChoiceQuestion("Do you need help with budgeting or managing your money?",
[
    Yes, 
    No
])
{
    public override string Code => nameof(C7);
    public const string Yes = "Yes";
    public const string No = "No";
}