namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.WellbeingAndMentalHealth;

public abstract class HealthCoreQuestion(string question) : SingleChoiceQuestion(question, [
    NotAtAll,
    OnlyOccasionally,
    Sometimes,
    Often,
    MostOrAllOfTheTime
])
{
    public const string NotAtAll = "Not at all";
    public const string OnlyOccasionally = "Only occasionally";
    public const string Sometimes = "Sometimes";
    public const string Often = "Often";
    public const string MostOrAllOfTheTime = "Most or all the time";
}