using System.Globalization;
using Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.HmppsRecords;

public static class HmppsDisplay
{
    public const string DateFormat = "dd/MM/yyyy";

    public static string? Format(DateOnly? date) => date?.ToString(DateFormat, CultureInfo.InvariantCulture);

    /// <summary>
    /// Licence dates are returned as strings; normalise ISO dates to the display format and pass anything else through.
    /// </summary>
    public static string? FormatLicenceDate(string? value) =>
        DateOnly.TryParse(value, CultureInfo.InvariantCulture, out var date) ? Format(date) : value;

    public static string JoinNames(params string?[] parts) =>
        string.Join(" ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));

    public static string FormatLength(SentenceLengthDto? length)
    {
        if (length is null)
        {
            return string.Empty;
        }

        if (length.Terms is { Count: > 0 } terms)
        {
            return string.Join("; ", terms.Select(FormatTerm).Where(t => t.Length > 0));
        }

        return length.Duration is { } duration
            ? $"{duration} {length.Units?.ToLowerInvariant()}".Trim()
            : string.Empty;
    }

    public static string FormatTerm(SentenceTermDto term)
    {
        var parts = new (int? Value, string Unit)[]
            {
                (term.Years, "y"), (term.Months, "m"), (term.Weeks, "w"), (term.Days, "d"), (term.Hours, "h")
            }
            .Where(p => p.Value is > 0)
            .Select(p => $"{p.Value}{p.Unit}");

        var duration = string.Join(" ", parts);
        var label = TermName(term.PrisonTermCode);

        return label is null ? duration : $"{label}: {duration}".TrimEnd(' ', ':');
    }

    public static string? TermName(string? code) => code switch
    {
        null or "" => null,
        "CUR" => "Curfew",
        "DEF" => "Deferment",
        "DET" => "Detention",
        "HOURS" => "Hours ordered",
        "IMP" => "Imprisonment",
        "LIC" => "Licence",
        "PSYCH" => "Psychiatric hospital",
        "SCUS" => "Custodial period",
        "SEC104" => "Breach of supervision",
        "SEC105" => "Breach (imprisonable offence)",
        "SEC86" => "Section 86",
        "SUP" => "Sentence length",
        "SUSP" => "Suspension period",
        _ => code
    };

    public static string LicenceTypeName(string? typeCode) => typeCode switch
    {
        "AP" => "Licence",
        "PSS" => "Post sentence supervision",
        "AP_PSS" => "Licence and post sentence supervision",
        null or "" => "Licence",
        _ => typeCode
    };

    /// <summary>
    /// Licences in force, or being varied while in force, are shown first and expanded.
    /// </summary>
    public static bool IsCurrentLicence(string? status) =>
        status is "ACTIVE" or "VARIATION_IN_PROGRESS" or "VARIATION_SUBMITTED" or "VARIATION_APPROVED" or "VARIATION_REJECTED";

    public static Color LicenceStatusColor(string? status) => status switch
    {
        "ACTIVE" => Color.Success,
        _ when IsCurrentLicence(status) => Color.Info,
        "INACTIVE" or "RECALLED" or "TIMED_OUT" => Color.Default,
        _ => Color.Warning
    };

    public static string Humanise(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value.Replace('_', ' ').ToLowerInvariant());
}
