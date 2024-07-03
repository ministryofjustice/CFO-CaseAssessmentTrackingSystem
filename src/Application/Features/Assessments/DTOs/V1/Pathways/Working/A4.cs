namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

public class A4()
    : SingleChoiceQuestion("Have you volunteered / provided unpaid help to a club, group, or charity in the last year?",
    [
        AtLeastOncePerMonth,
        LessThanOncePerMonth,
        NoCannot,
        NoPreferNot
    ])
{
    public const string AtLeastOncePerMonth = "Yes - at least once per month";
    public const string LessThanOncePerMonth = "Yes - less than once per month";
    public const string NoCannot = "No - I am currently unable to";
    public const string NoPreferNot = "No - I prefer to do other things";
}
