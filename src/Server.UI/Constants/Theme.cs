namespace Cfo.Cats.Server.UI.Constants;

public static class Theme
{
    public static MudTheme ApplicationTheme(string primaryColour, ThemeDarkColours primaryColourDark)
    {

        var myFont = new[] { "Arial", "Helvetica", "sans-serif" };

        var theme = new MudTheme()
        {
            PaletteLight = new PaletteLight
            {
                Primary = primaryColour,
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
                Primary = primaryColourDark.Primary,
                Secondary = "#9ECBE2",
                Info = "#9ECBE2",
                Tertiary = "#F1D08B",

                // Semantic colours
                Success = "#9BE06B",
                Error = "#FF99AF",

                AppbarBackground = primaryColourDark.AppbarBackground,
                AppbarText = "#FFFFFF",

                // Lines / tables / dividers
                TableLines = primaryColourDark.TableLines,
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
                        FontSize = "1rem",
                        LineHeight = "1.5"
                    },

                        // Use these for real page structure only
                        H1 = { FontFamily = myFont, FontSize = "1.625rem", FontWeight = "700", LineHeight = "1.25" },
                        H2 = { FontFamily = myFont, FontSize = "1.45rem",  FontWeight = "700", LineHeight = "1.3" },
                        H3 = { FontFamily = myFont, FontSize = "1.125rem", FontWeight = "600", LineHeight = "1.35" },
                        H4 = { FontFamily = myFont, FontSize = "1rem",     FontWeight = "600", LineHeight = "1.4" },
                        H5 = { FontFamily = myFont, FontSize = "1rem",     FontWeight = "500", LineHeight = "1.4" },
                        H6 = { FontFamily = myFont, FontSize = "0.875rem", FontWeight = "500", LineHeight = "1.4" },

                        // Use these for visual emphasis inside cards, panels, summaries
                        Subtitle1 =
                        {
                            FontFamily = myFont,
                            FontSize = "1rem",
                            FontWeight = "600",
                            LineHeight = "1.35",
                            LetterSpacing = "0.0025em",
                            TextTransform = "uppercase"
                        },

                        Subtitle2 =
                        {
                            FontFamily = myFont,
                            FontSize = "0.875rem",
                            FontWeight = "600",
                            LineHeight = "1.35",
                            LetterSpacing = "0.0025em"
                        },

                        Body1 =
                        {
                            FontFamily = myFont,
                            FontSize = "1rem",
                            FontWeight = "400",
                            LineHeight = "1.5"
                        },

                        Body2 =
                        {
                            FontFamily = myFont,
                            FontSize = "0.875rem",
                            FontWeight = "400",
                            LineHeight = "1.45"
                        },

                        Button =
                        {
                            FontFamily = myFont,
                            FontSize = "1rem",
                            FontWeight = "600",
                            LineHeight = "1.25",
                            LetterSpacing = "0.01em"
                        },

                        Caption =
                        {
                            FontFamily = myFont,
                            FontSize = "0.8125rem", // 13px, more distinct from body2
                            FontWeight = "400",
                            LineHeight = "1.35",
                            LetterSpacing = "0.01em"
                        },

                        Overline =
                        {
                        FontFamily = myFont,
                        FontSize = "0.75rem",
                        FontWeight = "600",
                        LineHeight = "1.3",
                        LetterSpacing = "0.06em",
                        TextTransform = "uppercase"
                    }
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
