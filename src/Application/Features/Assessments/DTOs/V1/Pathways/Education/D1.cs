namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Education;

public class D1() : SingleChoiceQuestion("What is your highest level of qualification?",
    "This could be how far you got in school/college or could be a qualification you got later.",
    [
        NoQualificationsOrEntryLevel,
        NvqLevelOneOrSimilar,
        FivePlusGcsesOrSimilar,
        TwoPlusALevelsOrSimilar,
        DegreeOrHigher
    ])
{
    public const string NoQualificationsOrEntryLevel = "No quals / entry level Example:Skills for life";
    public const string NvqLevelOneOrSimilar = "NVQ level 1 or similar Example:Grade D/3 or lower at GCSE";
    public const string FivePlusGcsesOrSimilar = "5+ GCSEs, NVQ level 2, etc. Example:Grade C/4 or higher at GCSE";
    public const string TwoPlusALevelsOrSimilar = "2+ A-levels, NVQ level 3, apprentice-ship, etc. Example:HNC";
    public const string DegreeOrHigher = "Degree or higher Example:BA";
}
