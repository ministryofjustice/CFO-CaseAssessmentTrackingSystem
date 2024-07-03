namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;

public class B5() : SingleChoiceQuestion(
    "How safe do/would you feel walking alone in your local area after dark?",
    [
        VeryUnsafeToWalkAloneAfterDarkInLocalArea,
        ABitUnsafeToWalkAloneAfterDarkInLocalArea,
        FairlySafeToWalkAloneAfterDarkInLocalArea,
        VerySafeToWalkAloneAfterDarkInLocalArea,
    ])
{
    public const string VeryUnsafeToWalkAloneAfterDarkInLocalArea = "Very unsafe";
    public const string ABitUnsafeToWalkAloneAfterDarkInLocalArea = "A bit unsafe";
    public const string FairlySafeToWalkAloneAfterDarkInLocalArea = "Fairly safe";
    public const string VerySafeToWalkAloneAfterDarkInLocalArea = "Very safe";
}