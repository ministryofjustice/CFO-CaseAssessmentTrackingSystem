namespace Cfo.Cats.Server.UI.Constants;

public static class Theme
{
    /// <summary>
    /// The default primary colour for the site.
    /// </summary>
    public const string DefaultPrimaryColour = "#6E3562";

    public static MudTheme ApplicationTheme(string primaryColour)
    {

        var myFont = new[] { "Arial", "Helvetica", "sans-serif" };

        var theme = new MudTheme()
        {
            PaletteLight = new PaletteLight
            {
                Primary = primaryColour, // the default CFO Purple
                Secondary = "#34586E",
                Success = "#62ae2f",
                Info = "#34586E",
                Black = "#000000",
                TableLines = primaryColour,
                Tertiary = "#DFB160",
                AppbarBackground = primaryColour,
                AppbarText = "#FFFFFF",
                Error = "#CC0033",
                White = "#FFFFFF",
            },

            PaletteDark = new PaletteDark
            {
                // keep your brand colors...
                Primary = "#8A437A",
                Secondary = "#34586E",
                Success = "#76c043",
                Info = "#1493A3",
                Error = "#FF4D4D",
                Tertiary = "#DFB160",

                // critical for dialog readability:
                Background = "#121212",     // page background
                Surface = "#1E1E1E",        // dialog/paper surfaces (slightly lighter than Background)
                
                TextPrimary = "#E6E6E6",
                TextSecondary = "#B3B3B3",

                Divider = "rgba(255,255,255,0.12)",
                LinesDefault = "rgba(255,255,255,0.12)",

                OverlayDark = "rgba(0,0,0,0.60)",

                // keep these sane
                White = "#FFFFFF",
                Black = "#000000",

                PrimaryContrastText = "#FFFFFF",
            },
            Typography = new Typography
            {
                Default = new DefaultTypography
                {
                    FontFamily = myFont,
                    FontSize = "1rem" // 16px
                },

                H1 = { FontFamily = myFont, FontSize = "1.625rem", FontWeight = "700" }, // 26px
                H2 = { FontFamily = myFont, FontSize = "1.25rem",  FontWeight = "700" }, // 20px
                H3 = { FontFamily = myFont, FontSize = "1.125rem", FontWeight = "600" }, // 18px
                H4 = { FontFamily = myFont, FontSize = "1rem",     FontWeight = "600" }, // 16px
                H5 = { FontFamily = myFont, FontSize = "1rem",     FontWeight = "500" }, // 16px
                H6 = { FontFamily = myFont, FontSize = "0.875rem", FontWeight = "500" }, // 14px

                Subtitle1 = { FontFamily = myFont, FontSize = "1rem",     FontWeight = "400" }, // 16px
                Subtitle2 = { FontFamily = myFont, FontSize = "0.875rem", FontWeight = "500" }, // 14px

                Body1 = { FontFamily = myFont, FontSize = "1rem",     FontWeight = "400" }, // 16px
                Body2 = { FontFamily = myFont, FontSize = "0.875rem", FontWeight = "400" }, // 14px

                Button = { FontFamily = myFont, FontSize = "1rem", FontWeight = "600" }, // 16px

                Caption = { FontFamily = myFont, FontSize = "0.875rem", FontWeight = "400" }, // 14px
                Overline = { FontFamily = myFont, FontSize = "0.75rem", FontWeight = "500" }  // 12px
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
