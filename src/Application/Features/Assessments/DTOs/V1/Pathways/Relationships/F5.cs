namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class F5() : SingleChoiceQuestion("How often do you see family or close friends in person?",
    [
        AtleastOncePerWeek,
        LessThanOncePerWeek,
        RarelyOrNever,
    ])
{
    public const string AtleastOncePerWeek = "At least once per week";
    public const string LessThanOncePerWeek = "Less than once per week";
    public const string RarelyOrNever = "Rarely or never";
}