namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;

public class E8() : SingleChoiceQuestion("Are you a smoker?",
    [
        Yes,
        IUsedTo,
        NeverSmoked
    ])
{
    public const string Yes = "Yes";
    public const string IUsedTo = "I used to";
    public const string NeverSmoked = "Never smoked";
}