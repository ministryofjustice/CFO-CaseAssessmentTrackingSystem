using Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;
using Cfo.Cats.Application.Features.HmppsExternalApi.Queries;
using static Cfo.Cats.Server.UI.Pages.Risk.RiskComponents.HmppsRiskDisplay;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;

public partial class TierRiskDetail : CatsComponent<TierDetailsDto>
{
    [Parameter]
    [EditorRequired]
    public string ParticipantId { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public string Crn { get; set; } = default!;

    protected override IQuery<Result<TierDetailsDto>> CreateQuery() => new GetTierDetails.Query
    {
        ParticipantId = ParticipantId,
        Crn = Crn
    };

    private string? TierScore => Data?.TierScore ?? Data?.Data?.Tier;

    private bool IsProvisional => Data?.Provisional == true || Data?.Data?.Provisional == true;

    // Tiers run from A (highest risk / complexity) downwards; the first character determines the band.
    private string TierCssColor => TierScore?.ToUpperInvariant().FirstOrDefault() switch
    {
        'A' or 'B' => LevelCssColor("HIGH"),
        'C' or 'D' => LevelCssColor("MEDIUM"),
        null or '\0' => LevelCssColor(null),
        _ => LevelCssColor("LOW"),
    };

    private string? Mappa
    {
        get
        {
            var registrations = Data?.Data?.DeliusInputs?.Registrations;
            var parts = new[]
            {
                string.IsNullOrWhiteSpace(registrations?.MappaCategory) ? null : $"CAT {registrations.MappaCategory}",
                string.IsNullOrWhiteSpace(registrations?.MappaLevel) ? null : $"LEVEL {registrations.MappaLevel}",
            }.OfType<string>().ToArray();

            return parts.Length > 0 ? string.Join("/", parts) : null;
        }
    }

    private IEnumerable<(string Label, bool? Value)> RegistrationFlags =>
    [
        ("Lifer/IPP", Data?.Data?.DeliusInputs?.Registrations?.HasLiferIpp),
        ("Domestic abuse", Data?.Data?.DeliusInputs?.Registrations?.HasDomesticAbuse),
        ("Stalking", Data?.Data?.DeliusInputs?.Registrations?.HasStalking),
        ("Child protection", Data?.Data?.DeliusInputs?.Registrations?.HasChildProtection),
        ("IOM nominal", Data?.Data?.DeliusInputs?.Registrations?.HasIomNominal),
        ("Ever committed sexual offence", Data?.Data?.OasysInputs?.EverCommittedSexualOffence),
    ];

    private IEnumerable<string> ActiveFlags => RegistrationFlags.Where(f => f.Value == true).Select(f => f.Label);

    private IEnumerable<string> InactiveFlags => RegistrationFlags.Where(f => f.Value == false).Select(f => f.Label);

    internal static IEnumerable<(string Label, TierPredictorDto? Predictor)> PredictorItems(TierPredictorOutputDto? output) =>
    [
        ("Combined serious reoffending (CSRP)", output?.CombinedSeriousReoffendingPredictor),
        ("All reoffending", output?.AllReoffendingPredictor),
        ("Violent reoffending", output?.ViolentReoffendingPredictor),
        ("Serious violent reoffending", output?.SeriousViolentReoffendingPredictor),
        ("Direct contact sexual reoffending", output?.DirectContactSexualReoffendingPredictor),
        ("Indirect image contact sexual reoffending", output?.IndirectImageContactSexualReoffendingPredictor),
    ];

    // One alert per predictor that has a band, as per the original Tier header alerts.
    private IEnumerable<(string Label, string Band)> PredictorAlerts =>
        PredictorItems(Data?.Data?.OasysInputs?.Predictors?.Output)
            .Where(p => !string.IsNullOrWhiteSpace(p.Predictor?.Band))
            .Select(p => (p.Label, p.Predictor!.Band!));

    private static Severity BandSeverity(string? band) => band?.ToUpperInvariant() switch
    {
        "VERY_HIGH" or "HIGH" => Severity.Error,
        "MEDIUM" => Severity.Warning,
        "LOW" => Severity.Success,
        _ => Severity.Info,
    };

    private async Task ShowCalculation()
    {
        var parameters = new DialogParameters<TierCalculationDialog>
        {
            { x => x.Crn, Crn },
            { x => x.Tier, Data! }
        };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true,
        };

        await DialogService.ShowAsync<TierCalculationDialog>($"Tier calculation for CRN {Crn}", parameters, options);
    }
}
