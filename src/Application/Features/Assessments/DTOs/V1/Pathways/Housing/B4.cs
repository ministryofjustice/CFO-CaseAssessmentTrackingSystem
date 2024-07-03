namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;

public class B4() : SingleChoiceQuestion(
    "How satisfied are you with your current housing?",
    "If you’re in prison, then think about your expected housing on release.",
    [
        VeryDissatisfied,
        SlightlyDissatisfied,
        NeitherSatisfiedOrDissatisfied,
        FairlySatisfied,
        VerySatisfied,
    ])
{
    public const string VeryDissatisfied = "Very dissatisfied";
    public const string SlightlyDissatisfied = "Slightly dissatisfied";
    public const string NeitherSatisfiedOrDissatisfied = "Neither satisfied or dissatisfied";
    public const string FairlySatisfied = "Fairly satisfied";
    public const string VerySatisfied = "Very satisfied";
}