namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;

public class E9() : SingleChoiceQuestion("Are you a vaper?",
    [
        Yes,
        IUsedTo,
        NeverVaped
    ])
{
    public const string Yes = "Yes";
    public const string IUsedTo = "I used to";
    public const string NeverVaped = "Never vaped";
}