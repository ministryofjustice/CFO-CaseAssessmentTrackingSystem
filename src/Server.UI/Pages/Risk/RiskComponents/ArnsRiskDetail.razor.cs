using Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;
using Cfo.Cats.Application.Features.HmppsExternalApi.Queries;
using static Cfo.Cats.Server.UI.Pages.Risk.RiskComponents.HmppsRiskDisplay;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;

public partial class ArnsRiskDetail : CatsComponent<RisksDto>
{
    private static readonly string NotFoundMessage = new NotFoundResult<RisksDto>().ErrorMessage;

    private bool _notFound;

    [Parameter]
    [EditorRequired]
    public string ParticipantId { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public string Crn { get; set; } = default!;

    protected override IQuery<Result<RisksDto>> CreateQuery() => new GetArnsRisk.Query
    {
        ParticipantId = ParticipantId,
        Crn = Crn
    };

    protected override void OnError(string errorMessage)
    {
        base.OnError(errorMessage);
        _notFound = errorMessage == NotFoundMessage;
    }

    private sealed record Concern(string Label, string Summary, List<(string Heading, string Text)> Notes);

    private IEnumerable<Concern> RiskToThemselves
    {
        get
        {
            var self = Data?.RiskToSelf;
            yield return PairedConcern("Risk of suicide or self-harm", self?.Suicide, "suicide", self?.SelfHarm, "self-harm");
            yield return PairedConcern("Coping in custody or a hostel", self?.Custody, "coping in custody", self?.HostelSetting, "coping in a hostel");
            yield return PairedConcern("Vulnerability", self?.Vulnerability, "vulnerability", null, null);
        }
    }

    // Mirrors the MPOP wording for risk to themselves, e.g. "Immediate concerns about suicide and previous concerns about self-harm".
    private static Concern PairedConcern(string label, RiskDetailDto? a, string aName, RiskDetailDto? b, string? bName)
    {
        var curA = IsYes(a?.Current);
        var prevA = IsYes(a?.Previous);
        var curB = IsYes(b?.Current);
        var prevB = IsYes(b?.Previous);

        var summary = (curA, curB, prevA, prevB) switch
        {
            (true, true, _, _) => $"Immediate concerns about {aName} and {bName}",
            (true, false, _, true) => $"Immediate concerns about {aName} and previous concerns about {bName}",
            (false, true, true, _) => $"Immediate concerns about {bName} and previous concerns about {aName}",
            (true, false, _, false) => $"Immediate concerns about {aName}",
            (false, true, false, _) => $"Immediate concerns about {bName}",
            (false, false, true, true) => $"Previous concerns about {aName} and {bName}",
            (false, false, true, false) => $"Previous concerns about {aName}",
            (false, false, false, true) => $"Previous concerns about {bName}",
            _ when a?.Current is null && a?.Previous is null && b?.Current is null && b?.Previous is null => NotRecorded,
            _ => "No concerns",
        };

        var notes = new List<(string, string)>();
        AddNote(notes, "Current circumstances, issues and needs", aName, a?.CurrentConcernsText, bName, b?.CurrentConcernsText);
        AddNote(notes, "Previous circumstances, issues and needs", aName, a?.PreviousConcernsText, bName, b?.PreviousConcernsText);

        return new Concern(label, summary, notes);
    }

    private static void AddNote(List<(string, string)> notes, string heading, string aName, string? aText, string? bName, string? bText)
    {
        // Both sides of a pair can carry notes; label each when both are present so neither is hidden.
        var bothPresent = !string.IsNullOrWhiteSpace(aText) && !string.IsNullOrWhiteSpace(bText);

        if (!string.IsNullOrWhiteSpace(aText))
        {
            notes.Add((bothPresent ? $"{heading} ({aName})" : heading, aText));
        }

        if (!string.IsNullOrWhiteSpace(bText))
        {
            notes.Add((bothPresent ? $"{heading} ({bName})" : heading, bText));
        }
    }

    private static bool IsYes(string? value) => string.Equals(value, "YES", StringComparison.OrdinalIgnoreCase);

    private static string AnswerText(string? value) => value?.ToUpperInvariant() switch
    {
        "YES" => "Yes",
        "NO" => "No",
        "DK" => "Don't know",
        null => NotRecorded,
        _ => value,
    };

    private IEnumerable<(string Label, string? Value)> OtherRiskItems =>
    [
        ("Escape or abscond", Data?.OtherRisks?.EscapeOrAbscond),
        ("Control issues / disruptive behaviour", Data?.OtherRisks?.ControlIssuesDisruptiveBehaviour),
        ("Breach of trust", Data?.OtherRisks?.BreachOfTrust),
        ("Risk to other prisoners", Data?.OtherRisks?.RiskToOtherPrisoners),
    ];

    // Community categories grouped under each level, most severe first (e.g. "High risk: Children, Known adult").
    private IEnumerable<(string Level, string[] Categories)> CommunityRisksByLevel =>
        Levels
            .Select(l => (l, (Data?.Summary?.RiskInCommunity ?? [])
                .Where(kv => string.Equals(kv.Value, l, StringComparison.OrdinalIgnoreCase))
                .Select(kv => Humanise(kv.Key)!)
                .Order()
                .ToArray()))
            .Where(g => g.Item2.Length > 0);

    // Union of category keys across both dictionaries, so a category present in only one is still shown.
    private IEnumerable<string> RiskCategories =>
        (Data?.Summary?.RiskInCommunity?.Keys ?? Enumerable.Empty<string>())
            .Concat(Data?.Summary?.RiskInCustody?.Keys ?? Enumerable.Empty<string>())
            .Distinct()
            .Order();

    private static string? GetRiskLevel(Dictionary<string, string>? risks, string category) =>
        risks is not null && risks.TryGetValue(category, out var level) ? level : null;

    private static Color RiskLevelColor(string? level) => level switch
    {
        "VERY_HIGH" or "HIGH" => Color.Error,
        "MEDIUM" or "LOW" => Color.Warning,
        _ => Color.Default,
    };
}
