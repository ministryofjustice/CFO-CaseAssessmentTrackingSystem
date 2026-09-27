namespace Cfo.Cats.Server.UI.Components.Dashboard.QA;

/// <summary>
/// Provides a consistent, deterministic colour for each feedback reason across charts.
/// </summary>
public static class FeedbackChartColourProvider
{
    private const string DefaultColour = "#888888";

    private static readonly Dictionary<string, string> ReasonColours = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Positive Recognition"] = "#00E396",            // Green
        ["Information incomplete"] = "#008FFB",          // Blue
        ["Incorrect / Missing Paperwork"] = "#FEB019",   // Amber
        ["Information conflicts with CATS"] = "#775DD0", // Purple
        ["Ineligible Claim"] = "#FF4560",                // Red
        ["Other"] = "#546E7A",                           // Slate grey
        ["Unknown"] = DefaultColour
    };

    /// <summary>
    /// Returns a consistent colour for the given feedback reason.
    /// </summary>
    public static string GetColour(string? reason)
    {
        if (!string.IsNullOrWhiteSpace(reason) && ReasonColours.TryGetValue(reason.Trim(), out var colour))
        {
            return colour;
        }

        return DefaultColour;
    }
}
