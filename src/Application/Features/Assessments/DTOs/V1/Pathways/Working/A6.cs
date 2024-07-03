namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

public class A6() : SingleChoiceQuestion("Are you able to fill in application forms and write a CV?",
[
    YesUnaided,
    YesWithHelp,
    No
])
{
    public const string YesUnaided = "Yes, unaided";
    public const string YesWithHelp = "Yes, with help";
    public const string No = "No";
}