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
                // Dark neutrals
                Background = "#211B25",
                Surface = "#2B2430",
                DrawerBackground = "#1D1720",

                // Text
                TextPrimary = "#F7F2F6",
                TextSecondary = "#D7CBD5",

                // Brand colours - lightened for dark backgrounds
                Primary = "#D7A3CB",      // accessible tint of CFO Purple
                Secondary = "#9ECBE2",    // accessible tint of your blue
                Info = "#9ECBE2",
                Tertiary = "#F1D08B",     // accessible tint of gold

                // Semantic colours
                Success = "#9BE06B",
                Error = "#FF6B8A",

                AppbarBackground = "#6E3562",
                AppbarText = "#FFFFFF",

                // Lines / tables / dividers
                TableLines = "#766370",
                Divider = "#766370",
                DividerLight = "#4A3B48",

                // Base colours
                Black = "#000000",
                White = "#FFFFFF",

                PrimaryContrastText = "#1A1118",
                SecondaryContrastText = "#071820",
                TertiaryContrastText = "#211600",
                SuccessContrastText = "#102000",
                InfoContrastText = "#071820",
                ErrorContrastText = "#240008"
            },
            Typography = new Typography
            {
                Default = new DefaultTypography
                {
                    FontFamily = myFont,
                    FontSize = "1rem" // 16px
                },

                H1 = { FontFamily = myFont, FontSize = "1.625rem", FontWeight = "700" }, // 26px
                H2 = { FontFamily = myFont, FontSize = "1.45rem", FontWeight = "700" }, // 20px
                H3 = { FontFamily = myFont, FontSize = "1.125rem", FontWeight = "600" }, // 18px
                H4 = { FontFamily = myFont, FontSize = "1rem", FontWeight = "600" }, // 16px
                H5 = { FontFamily = myFont, FontSize = "1rem", FontWeight = "500" }, // 16px
                H6 = { FontFamily = myFont, FontSize = "0.875rem", FontWeight = "500" }, // 14px

                Subtitle1 = { FontFamily = myFont, FontSize = "1rem", FontWeight = "400" }, // 16px
                Subtitle2 = { FontFamily = myFont, FontSize = "0.875rem", FontWeight = "500" }, // 14px

                Body1 = { FontFamily = myFont, FontSize = "1rem", FontWeight = "400" }, // 16px
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
