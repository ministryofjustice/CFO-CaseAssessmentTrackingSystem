namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class F2() : SingleChoiceQuestion("How often do you feel lonely?",
    [
        OftenOrAlways,
        Sometimes,
        Occasionally,
        HardlyEver,
        Never,
    ])
{
    public const string OftenOrAlways = "Often or always";
    public const string Sometimes = "Some of the time";
    public const string Occasionally = "Occasionally";
    public const string HardlyEver = "Hardly ever";
    public const string Never = "Never";
}
