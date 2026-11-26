using Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;
using static Cfo.Cats.Server.UI.Pages.Risk.RiskComponents.HmppsRiskDisplay;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;

/// <summary>
/// Presentational dialog showing the full tier calculation for a single CRN. It is given the already loaded
/// <see cref="TierDetailsDto"/> for that CRN and never loads data itself.
/// </summary>
public partial class TierCalculationDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter, EditorRequired] public string Crn { get; set; } = default!;

    [Parameter, EditorRequired] public TierDetailsDto Tier { get; set; } = default!;

    private void Close() => MudDialog.Close();

    private static IEnumerable<(string Label, string Value)> DeliusItems(TierDeliusInputsDto delius) =>
    [
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
    ];

    private static IEnumerable<(string Label, TierAssessmentSectionDto? Section)> AssessmentSections(TierAssessmentSummaryDto summary) =>
    [
        ("Accommodation", summary.Accommodation),
        ("Education, training and employability", summary.EducationTrainingEmployability),
        ("Relationships", summary.Relationships),
        ("Lifestyle and associates", summary.LifestyleAndAssociates),
        ("Drug misuse", summary.DrugMisuse),
        ("Alcohol misuse", summary.AlcoholMisuse),
        ("Thinking and behaviour", summary.ThinkingAndBehaviour),
        ("Attitudes", summary.Attitudes),
    ];

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
}
