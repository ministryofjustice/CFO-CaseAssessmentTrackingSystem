using System.Globalization;
using System.Text.RegularExpressions;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;

/// <summary>
/// Shared formatting for HMPPS risk data (ARNS / Tier), so levels and bands are presented consistently.
/// </summary>
public static partial class HmppsRiskDisplay
{
    public const string NotRecorded = "Not recorded";

    public const string NoDetailGiven = "No detail given";

    /// <summary>
    /// Risk levels in descending order of severity, as returned by the HMPPS APIs.
    /// </summary>
    public static readonly string[] Levels = ["VERY_HIGH", "HIGH", "MEDIUM", "LOW"];

    public static string Show(string? value) => string.IsNullOrWhiteSpace(value) ? NotRecorded : value;

    public static string Show(bool? value) => value is null ? NotRecorded : value.Value ? "Yes" : "No";

    public static string Show(decimal? value) => value?.ToString("0.##", CultureInfo.InvariantCulture) ?? NotRecorded;

    public static string Show(DateTime? value) => value?.ToString("dd/MM/yyyy HH:mm") ?? NotRecorded;

    public static string Show(DateOnly? value) => value?.ToString("dd/MM/yyyy") ?? NotRecorded;

    // Converts API codes ("VERY_HIGH", "permanenceOfAccommodation") into readable text ("Very high", "Permanence of accommodation").
    public static string? Humanise(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var spaced = WordBoundary().Replace(value.Replace('_', ' '), " ").ToLowerInvariant();
        return char.ToUpperInvariant(spaced[0]) + spaced[1..];
    }

    public static string? Upper(string? value) => Humanise(value)?.ToUpperInvariant();

    public static Color LevelColor(string? level) => level?.ToUpperInvariant() switch
    {
        "VERY_HIGH" or "HIGH" => Color.Error,
        "MEDIUM" => Color.Warning,
        "LOW" => Color.Success,
        _ => Color.Default,
    };

    /// <summary>
    /// Css variable for the palette colour of a level, used for MPOP-style coloured widget borders.
    /// </summary>
    public static string LevelCssColor(string? level) => LevelColor(level) switch
    {
        Color.Error => "var(--mud-palette-error)",
        Color.Warning => "var(--mud-palette-warning)",
        Color.Success => "var(--mud-palette-success)",
        _ => "var(--mud-palette-lines-default)",
    };

    public static Color AnswerColor(string? value) => value?.ToUpperInvariant() switch
    {
        "YES" => Color.Error,
        "NO" => Color.Success,
        "DK" => Color.Warning,
        _ => Color.Default,
    };

    public static Color FlagColor(bool? value) => value switch
    {
        true => Color.Error,
        false => Color.Success,
        _ => Color.Default,
    };

    [GeneratedRegex("(?<=[a-z0-9])(?=[A-Z])")]
    private static partial Regex WordBoundary();
}
