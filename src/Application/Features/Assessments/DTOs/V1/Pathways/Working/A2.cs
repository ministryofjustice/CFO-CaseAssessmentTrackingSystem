namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

public class A2() : SingleChoiceQuestion(
"When were you last in work?",
[
    NeverWorked,
    OverAYearAgo,
    InTheLastYear,
    CurrentlyWorking
])
{
    public const string NeverWorked = "I have never worked";
    public const string OverAYearAgo = "Over a year ago";
    public const string InTheLastYear = "In the last year";
    public const string CurrentlyWorking = "I am currently working";
}