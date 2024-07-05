namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Money;

public class C9() : SingleChoiceQuestion(
    "Have you ever used a food bank?", 
    "If you’re in prison, think about the time before you came into prison.",
    [
        Never,
        OverYearAgo,
        WithinTheLastYear,
        WithinTheLast30Days
    ]
)
{
    public const string Never = "Never";
    public const string OverYearAgo = "Over a year ago";
    public const string WithinTheLastYear = "Within the last year";
    public const string WithinTheLast30Days = "Within the last 30 days";
}
