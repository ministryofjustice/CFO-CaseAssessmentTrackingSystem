namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class H8() : SingleChoiceQuestion("I have felt I have someone to turn to for support when needed",
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
