namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

public class A3() : SingleChoiceQuestion("Does or would your offence limit the types of work you could do?",
[
    Yes,
    No,
    NotSure
])
{
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NotSure = "Not Sure";
};