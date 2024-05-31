using System.Globalization;

namespace Cfo.Cats.Server.UI.Services.UserPreferences;

public class UserPreferences
{
    /// <summary>
    ///     Set the direction layout of the docs to RTL or LTR. If true RTL is used
    /// </summary>
    public bool RightToLeft { get; set; }

    /// <summary>
    ///     If true DarkTheme is used. LightTheme otherwise
    /// </summary>
    public bool IsDarkMode { get; set; }

    public string SecondaryColor { get; set; } = "#494f56";
    public double BorderRadius { get; set; } = 4;
    public double DefaultFontSize { get; set; } = 1;

    public double Body1FontSize => DefaultFontSize;
    public double Body2FontSize => DefaultFontSize - 0.125;
    public double ButtonFontSize => DefaultFontSize;
    public double CaptionFontSize => DefaultFontSize + 0.0625;
    public double OverlineFontSize => DefaultFontSize - 0.125;
    public double Subtitle1FontSize => DefaultFontSize + 0.125;
    public double Subtitle2FontSize => DefaultFontSize;
    public DarkLightMode DarkLightTheme { get; set; }

    private string AdjustBrightness(string hexColor, double factor)
    {
        if (hexColor.StartsWith("#"))
        {
            hexColor = hexColor.Substring(1);
        }

        if (hexColor.Length != 6)
        {
            throw new ArgumentException("Invalid hex color code. It must be 6 characters long.");
        }

        var r = int.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber);
        var g = int.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber);
        var b = int.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber);

        var newR = (int)Math.Clamp(r * factor, 0, 255);
        var newG = (int)Math.Clamp(g * factor, 0, 255);
        var newB = (int)Math.Clamp(b * factor, 0, 255);

        return $"#{newR:X2}{newG:X2}{newB:X2}";
    }
}

public enum DarkLightMode
{
    System = 0,
    Light = 1,
    Dark = 2
}
