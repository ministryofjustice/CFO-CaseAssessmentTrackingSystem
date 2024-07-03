namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Money;

public class C8()
 : SingleChoiceQuestion("Have you used a food bank?", [
     Never,
     OverYearAgo,
     WithinLastYear,
     WithinLast30Days
 ])
{
    public const string Never = "Never";
    public const string OverYearAgo = "Over a year ago";
    public const string WithinLastYear = "Within the last year";
    public const string WithinLast30Days = "Within the last 30 days";
}