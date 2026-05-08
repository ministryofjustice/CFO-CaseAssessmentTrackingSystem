namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

public class A10() : SingleChoiceQuestion("Do you or your household have access to a motor vehicle?",
"E.g., a car, van, motorbike, moped etc.",
[
    Yes,
    No,
])
{
    public override string Code => nameof(A10);
    public const string Yes = "Yes";
    public const string No = "No";
}