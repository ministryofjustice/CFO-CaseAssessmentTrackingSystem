namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

public class A7() : SingleChoiceQuestion("Are you comfortable using computers, tablets, iPads, laptops etc?",
[
    Yes,
    No
])
{
    public const string Yes = "Yes";
    public const string No = "No";
}