using Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;
using Cfo.Cats.Application.Features.HmppsExternalApi.Queries;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;

public partial class ArnsRiskDetail : CatsComponent<RisksDto>
{
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

    private IEnumerable<(string Label, RiskDetailDto? Detail)> RiskToSelfItems => new (string, RiskDetailDto?)[]
    {
        ("Suicide", Data?.RiskToSelf?.Suicide),
        ("Self harm", Data?.RiskToSelf?.SelfHarm),
        ("Custody", Data?.RiskToSelf?.Custody),
        ("Hostel setting", Data?.RiskToSelf?.HostelSetting),
        ("Vulnerability", Data?.RiskToSelf?.Vulnerability),
    };

    private IEnumerable<(string Label, string? Value)> OtherRiskItems => new (string, string?)[]
    {
        ("Escape or abscond", Data?.OtherRisks?.EscapeOrAbscond),
        ("Control issues / disruptive behaviour", Data?.OtherRisks?.ControlIssuesDisruptiveBehaviour),
        ("Breach of trust", Data?.OtherRisks?.BreachOfTrust),
        ("Risk to other prisoners", Data?.OtherRisks?.RiskToOtherPrisoners),
    };

    // Union of category keys across both dictionaries, so a category present in only one is still shown.
    private IEnumerable<string> RiskCategories =>
        (Data?.Summary?.RiskInCommunity?.Keys ?? Enumerable.Empty<string>())
            .Concat(Data?.Summary?.RiskInCustody?.Keys ?? Enumerable.Empty<string>())
            .Distinct()
            .OrderBy(c => c);

    private static string? GetRiskLevel(Dictionary<string, string>? risks, string category) =>
        risks is not null && risks.TryGetValue(category, out var level) ? level : null;

    private static Color RiskLevelColor(string? level) => level switch
    {
        "VERY_HIGH" or "HIGH" => Color.Error,
        "MEDIUM" or "LOW" => Color.Warning,
        _ => Color.Default,
    };

    private static Color RiskColor(string? value) => value switch
    {
        "YES" => Color.Error,
        "NO" => Color.Success,
        "DK" => Color.Warning,
        _ => Color.Default,
    };
}
