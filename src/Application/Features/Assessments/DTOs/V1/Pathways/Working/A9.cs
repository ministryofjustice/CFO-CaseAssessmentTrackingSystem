namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

public class A9() : SingleChoiceQuestion(
"Do you know how to disclose your offence?",
[
    Yes,
    No
]
)
{
    public override string Code => nameof(A9);
    public const string Yes = "Yes";
    public const string No = "No";
};