namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Education;
public class D4() : MultipleChoiceQuestion("Do you have difficulty with any of the following? (tick all that apply)",
    "Try to think if they affect your day-to-day life in a negative way.",
    [
        ReadingDifficulty,
        CoordinationDifficulty,
        WritingDifficulty,
        SpeakingDifficulty,
        UsingNumbersDifficulty,
        ConcentratingDifficulty,
        MemoryDifficulty,
        UnderstandingOthersDifficulty,
        SittingStillDifficulty,
        OtherDifficulties
    ])
{
    public const string ReadingDifficulty = "Reading";
    public const string CoordinationDifficulty = "Coordination";
    public const string WritingDifficulty = "Writing";
    public const string SpeakingDifficulty = "Speaking";
    public const string UsingNumbersDifficulty = "Using numbers";
    public const string ConcentratingDifficulty = "Concentrating";
    public const string MemoryDifficulty = "Memory";
    public const string UnderstandingOthersDifficulty = "Understanding others";
    public const string SittingStillDifficulty = "Restlessness / sitting still";
    public const string OtherDifficulties = "Others";
}
