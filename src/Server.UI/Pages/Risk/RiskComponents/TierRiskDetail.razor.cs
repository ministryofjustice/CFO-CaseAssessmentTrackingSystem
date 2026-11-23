using System.Globalization;
using System.Text.RegularExpressions;
using Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;
using Cfo.Cats.Application.Features.HmppsExternalApi.Queries;

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

    private static Severity RiskLevelSeverity(string? level) => level switch
    {
        "VERY_HIGH" or "HIGH" => Severity.Error,
        "MEDIUM" or "LOW" => Severity.Warning,
        _ => Severity.Info,
    };

    private string? TierScore => Data?.TierScore ?? Data?.Data?.Tier;

    private bool IsProvisional => Data?.Provisional == true || Data?.Data?.Provisional == true;

    private IEnumerable<(string Label, bool? Value)> RegistrationFlags => new (string, bool?)[]
    {
        ("Lifer/IPP", Data?.Data?.DeliusInputs?.Registrations?.HasLiferIpp),
        ("Domestic abuse", Data?.Data?.DeliusInputs?.Registrations?.HasDomesticAbuse),
        ("Stalking", Data?.Data?.DeliusInputs?.Registrations?.HasStalking),
        ("Child protection", Data?.Data?.DeliusInputs?.Registrations?.HasChildProtection),
        ("IOM nominal", Data?.Data?.DeliusInputs?.Registrations?.HasIomNominal),
        ("Ever committed sexual offence", Data?.Data?.OasysInputs?.EverCommittedSexualOffence),
    };

    private static IEnumerable<(string Label, string Value)> DeliusItems(TierDeliusInputsDto delius) => new (string, string)[]
    {
        ("Female", Show(delius.IsFemale)),
        ("RSR score", Show(delius.RsrScore)),
        ("OGRS score", Show(delius.OgrsScore)),
        ("No mandate", Show(delius.HasNoMandate)),
        ("Active event", Show(delius.HasActiveEvent)),
        ("Previous enforcement activity", Show(delius.PreviousEnforcementActivity)),
        ("Unsupervised", Show(delius.Registrations?.Unsupervised)),
        ("Latest release date", Show(delius.LatestReleaseDate)),
        ("Latest Sentencing Act 2026 excluded offence date", Show(delius.LatestSentencingAct2026ExcludedOffenceDate)),
        ("Latest child sexual exploitation sentence date", Show(delius.LatestChildSexualExploitationSentenceDate)),
    };

    private IEnumerable<(string Label, TierAssessmentSectionDto? Section)> AssessmentSections => new (string, TierAssessmentSectionDto?)[]
    {
        ("Accommodation", Data?.Data?.AssessmentSummary?.Accommodation),
        ("Education, training and employability", Data?.Data?.AssessmentSummary?.EducationTrainingEmployability),
        ("Relationships", Data?.Data?.AssessmentSummary?.Relationships),
        ("Lifestyle and associates", Data?.Data?.AssessmentSummary?.LifestyleAndAssociates),
        ("Drug misuse", Data?.Data?.AssessmentSummary?.DrugMisuse),
        ("Alcohol misuse", Data?.Data?.AssessmentSummary?.AlcoholMisuse),
        ("Thinking and behaviour", Data?.Data?.AssessmentSummary?.ThinkingAndBehaviour),
        ("Attitudes", Data?.Data?.AssessmentSummary?.Attitudes),
    };

    // Question answers plus the section-specific extra fields, which are only populated on some sections.
    private static IEnumerable<(string Question, string? Answer)> SectionAnswers(TierAssessmentSectionDto section)
    {
        foreach (var (key, value) in section.QuestionAnswers ?? [])
        {
            yield return (Humanise(key)!, value);
        }

        if (section.ParentalResponsibilities is not null)
        {
            yield return ("Parental responsibilities", section.ParentalResponsibilities);
        }

        if (section.Impulsivity is not null)
        {
            yield return ("Impulsivity", section.Impulsivity);
        }

        if (section.TemperControl is not null)
        {
            yield return ("Temper control", section.TemperControl);
        }
    }

    private static IEnumerable<(string Label, TierPredictorDto? Predictor)> PredictorItems(TierPredictorOutputDto? output) => new (string, TierPredictorDto?)[]
    {
        ("All reoffending", output?.AllReoffendingPredictor),
        ("Violent reoffending", output?.ViolentReoffendingPredictor),
        ("Serious violent reoffending", output?.SeriousViolentReoffendingPredictor),
        ("Direct contact sexual reoffending", output?.DirectContactSexualReoffendingPredictor),
        ("Indirect image contact sexual reoffending", output?.IndirectImageContactSexualReoffendingPredictor),
        ("Combined serious reoffending", output?.CombinedSeriousReoffendingPredictor),
    };

    private static string FormatMappa(TierRegistrationsDto? registrations)
    {
        var parts = new[]
        {
            string.IsNullOrWhiteSpace(registrations?.MappaCategory) ? null : $"Cat {registrations.MappaCategory}",
            string.IsNullOrWhiteSpace(registrations?.MappaLevel) ? null : $"Level {registrations.MappaLevel}",
        }.Where(p => p is not null);

        var formatted = string.Join("/", parts);
        return formatted.Length > 0 ? formatted : "Not recorded";
    }

    private static string Show(string? value) => string.IsNullOrWhiteSpace(value) ? "Not recorded" : value;

    private static string Show(bool? value) => value is null ? "Not recorded" : value.Value ? "Yes" : "No";

    private static string Show(decimal? value) => value?.ToString("0.##", CultureInfo.InvariantCulture) ?? "Not recorded";

    private static string Show(DateTime? value) => value?.ToString("dd/MM/yyyy HH:mm") ?? "Not recorded";

    private static string Show(DateOnly? value) => value?.ToString("dd/MM/yyyy") ?? "Not recorded";

    // Converts API codes ("VERY_HIGH", "permanenceOfAccommodation") into readable text ("Very high", "Permanence of accommodation").
    private static string? Humanise(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var spaced = Regex.Replace(value.Replace('_', ' '), "(?<=[a-z0-9])(?=[A-Z])", " ").ToLowerInvariant();
        return char.ToUpperInvariant(spaced[0]) + spaced[1..];
    }

    private static Color TierColor(string? tier) => tier?.ToUpperInvariant() switch
    {
        "A" or "B" => Color.Error,
        "C" or "D" => Color.Warning,
        null or "" => Color.Default,
        _ => Color.Success,
    };

    private static Color BandColor(string? band) => band?.ToUpperInvariant() switch
    {
        "VERY_HIGH" or "HIGH" => Color.Error,
        "MEDIUM" => Color.Warning,
        "LOW" => Color.Success,
        _ => Color.Default,
    };

    private static Color FlagColor(bool? value) => value switch
    {
        true => Color.Error,
        false => Color.Success,
        _ => Color.Default,
    };

    private static Color FlagColor(string? value) => value?.ToUpperInvariant() switch
    {
        "YES" => Color.Error,
        "NO" => Color.Success,
        _ => Color.Default,
    };

}
