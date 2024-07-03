namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;

public class B4() : SingleChoiceQuestion(
    "How satisfied are you with your current housing?",
    "If you’re in prison, then think about your expected housing on release.",
    [
        VeryDissatisfiedHousing,
        SlightlySatisfiedHousing,
        NeitherSatisfiedOrDissatisfiedHousing,
        FairlySatisfiedHousing,
        VerySatisfiedHousing,
    ])
{
    public const string VeryDissatisfiedHousing = "Very dissatisfied";
    public const string SlightlySatisfiedHousing = "Slightly dissatisfied";
    public const string NeitherSatisfiedOrDissatisfiedHousing = "Neither satisfied or dissatisfied";
    public const string FairlySatisfiedHousing = "Fairly satisfied";
    public const string VerySatisfiedHousing = "Very satisfied";
}