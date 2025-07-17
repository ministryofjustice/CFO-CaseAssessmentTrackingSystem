namespace Cfo.Cats.Infrastructure.Configurations;

public class OutcomeQualityDipSampleSettings
{
    public const string Key = nameof(OutcomeQualityDipSampleSettings);

    public int Wing { get; set; } = 10;
    public int Hub { get; set; } = 10;
    public int Satellite { get; set; } = 5;
    public int Community { get; set; } = 10;
    public int WiderCustody { get; set; } = 10;
}
