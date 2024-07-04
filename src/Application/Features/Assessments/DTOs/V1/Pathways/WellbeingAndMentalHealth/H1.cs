namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class H1() : SingleChoiceQuestion("Talking to people has felt too much for me",
    [
        NotAtAll,
        OnlyOccasionally,
        Sometimes,
        Often,
        MostOrAllTheTime
    ])
{
    public const string NotAtAll = "Not at all";
    public const string OnlyOccasionally = "Only occasionally";
    public const string Sometimes = "Sometimes";
    public const string Often = "Often";
    public const string MostOrAllTheTime = "Most or all the time";
}