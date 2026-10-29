using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;

public partial class ArnsRiskDetail : ComponentBase
{
    [Parameter]
    [EditorRequired]
    public IHmppsExternalApiService Service { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public string HmppsId { get; set; } = default!;

    private bool _loading = true;
    private string? _riskError;
    private RisksDto? _risks;

    protected override async Task OnInitializedAsync()
    {
        var result = await Service.GetSeriousHarmRiskAsync(HmppsId, CancellationToken.None);

        if (result.Succeeded)
        {
            _risks = result.Data;
        }
        else
        {
            _riskError = result.ErrorMessage is { Length: > 0 } ? result.ErrorMessage : "No risk data found.";
        }

        _loading = false;
    }

    private IEnumerable<(string Label, RiskDetailDto? Detail)> RiskToSelfItems => new (string, RiskDetailDto?)[]
    {
        ("Suicide", _risks?.RiskToSelf?.Suicide),
        ("Self harm", _risks?.RiskToSelf?.SelfHarm),
        ("Custody", _risks?.RiskToSelf?.Custody),
        ("Hostel setting", _risks?.RiskToSelf?.HostelSetting),
        ("Vulnerability", _risks?.RiskToSelf?.Vulnerability),
    };

    private IEnumerable<(string Label, string? Value)> OtherRiskItems => new (string, string?)[]
    {
        ("Escape or abscond", _risks?.OtherRisks?.EscapeOrAbscond),
        ("Control issues / disruptive behaviour", _risks?.OtherRisks?.ControlIssuesDisruptiveBehaviour),
        ("Breach of trust", _risks?.OtherRisks?.BreachOfTrust),
        ("Risk to other prisoners", _risks?.OtherRisks?.RiskToOtherPrisoners),
    };

    // Union of category keys across both dictionaries, so a category present in only one is still shown.
    private IEnumerable<string> RiskCategories =>
        (_risks?.Summary?.RiskInCommunity?.Keys ?? Enumerable.Empty<string>())
            .Concat(_risks?.Summary?.RiskInCustody?.Keys ?? Enumerable.Empty<string>())
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
