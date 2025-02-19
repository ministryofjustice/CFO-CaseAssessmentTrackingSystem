namespace Cfo.Cats.Server.UI.Constants;

public static class Theme
{
    /// <summary>
    /// The default primary colour for the site.
    /// </summary>
    public const string DefaultPrimaryColour = "#722660";

    public static MudTheme ApplicationTheme(string primaryColour)
    {
        var theme = new MudTheme()
        {
            PaletteLight = new PaletteLight
            {
                Primary = primaryColour, // the default CFO Purple
                Secondary = "#494f56",
                Success = "#62ae2f",
                Info = "#0c6980",
                Black = "#000000",
                TableLines = primaryColour,
                Tertiary = "#FFFFFF",
                AppbarBackground = primaryColour,
                AppbarText = "#FFFFFF",
                Error="#CC0033",
                White = "#FFFFFF",
            },
            PaletteDark = new PaletteDark
            {
                Primary = primaryColour, // Adjusted CFO Purple for dark mode
                Secondary = "#666B73", // Lighter than #494f56 for better contrast on dark backgrounds
                Success = "#76c043", // Slightly brighter than #62ae2f for visibility
                Info = "#1493A3", // Brighter than #0c6980 for better contrast
                Black = "#FFFFFF", // White to contrast dark backgrounds
                TableLines = primaryColour, // Same as Primary to maintain consistency
                Tertiary = "#FFFFFF", // Very dark grey for background contrast
                AppbarBackground = primaryColour, // Matching Tertiary for a unified dark theme
                AppbarText = "#FFFFFF",
                Error = "#FF4D4D", // Brighter red than #CC0033 for better visibility,
                White = "#FFFFFF",
                TextPrimary = "#FFFFFF",
            },
            Typography = new Typography
            {
                Body1 = new Body1Typography() { FontFamily = default, FontSize = "14px" },
                Body2 = new Body2Typography() //Custom body 2 typography with 16px size.
                {
                    FontFamily = default
                },
                Default = { FontFamily = ["Arial", "Helvetica", "sans-serif"] },
                H4 = { FontSize = "24px" }
            }
        };

        return theme;
    }

    public static class Tables
    {
        public const bool Dense = false;
        public const bool Striped = true;
    }
}
