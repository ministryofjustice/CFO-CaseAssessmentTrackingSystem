namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.WellbeingAndMentalHealth;

public class H11() : SingleChoiceQuestion("On a scale 1 to 5, overall how satisfied are you with your life?", [
    NotAtAll,
    MostlyNot,
    Fairly,
    Mostly,
    Completely
])
{
    public const string NotAtAll = "1 Not at all";
    public const string MostlyNot = "2 Mostly not";
    public const string Fairly  = "3 Fairly";
    public const string Mostly = "4 Mostly";
    public const string Completely =  "5 Completely";

}