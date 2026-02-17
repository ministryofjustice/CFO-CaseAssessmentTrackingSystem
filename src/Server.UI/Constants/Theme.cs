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
            }
,
            Typography = new Typography
            {
                Default = new DefaultTypography
                {
                    FontFamily = myFont,
                    FontSize = ".875rem"
                },
                H1 = { FontFamily = myFont, FontSize = "2.5rem", FontWeight = "300" },
                H2 = { FontFamily = myFont, FontSize = "2rem", FontWeight = "300" },
                H3 = { FontFamily = myFont, FontSize = "1.75rem", FontWeight = "400" },
                H4 = { FontFamily = myFont, FontSize = "1.5rem", FontWeight = "400" },
                H5 = { FontFamily = myFont, FontSize = "1.25rem", FontWeight = "400" },
                H6 = { FontFamily = myFont, FontSize = "1rem", FontWeight = "500" },
                Subtitle1 = { FontFamily = myFont, FontSize = ".875rem", FontWeight = "400" },
                Subtitle2 = { FontFamily = myFont, FontSize = ".8125rem", FontWeight = "500" },
                Body1 = { FontFamily = myFont, FontSize = ".875rem", FontWeight = "400" },
                Body2 = { FontFamily = myFont, FontSize = ".75rem", FontWeight = "400" },
                Button = { FontFamily = myFont, FontSize = ".75rem", FontWeight = "500" },
                Caption = { FontFamily = myFont, FontSize = ".6875rem", FontWeight = "400" },
                Overline = { FontFamily = myFont, FontSize = ".625rem", FontWeight = "400" }
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
