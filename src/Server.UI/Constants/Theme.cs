namespace Cfo.Cats.Server.UI.Constants;

public static class Theme
{
    public static MudTheme ApplicationTheme()
    {
        var theme = new MudTheme()
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#722660", // the default CFO Purple
                Secondary = "#494f56",
                Success = "#62ae2f",
                Info = "#0c6980",
                Black = "#000000",
                TableLines = "#722660",
                Tertiary = "#FFFFFF",
                AppbarBackground = "#722660"
            },
            PaletteDark = new PaletteDark
            {
                Primary = "#0092CC",
                Secondary = "#494f56",
                Success = "#62ae2f",
                Info = "#0c6980",
                Black = "#000000",
                TableLines = "#0092CC",
                Tertiary = "#FFFFFF",
                AppbarBackground = "#0092CC"
            },
            Typography = new Typography
            {
                Body1 = new Body1() { FontFamily = default, FontSize = "14px" },
                Body2 = new Body2() //Custom body 2 typography with 16px size.
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
