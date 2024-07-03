namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;
public class E1() : SingleChoiceQuestion("How is your health in general?", [
    VeryBad,
    Bad,
    Fair,
    Good,
    VeryGood,
])
{
    public const string VeryBad = "Very bad";
    public const string Bad = "Bad";
    public const string Fair = "Fair";
    public const string Good = "Good";
    public const string VeryGood = "Very good";
}
