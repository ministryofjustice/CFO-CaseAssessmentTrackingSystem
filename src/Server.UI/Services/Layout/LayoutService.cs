using System.Globalization;
using Cfo.Cats.Server.UI.Services.UserPreferences;

namespace Cfo.Cats.Server.UI.Services.Layout;

public class LayoutService
{
    private readonly IUserPreferencesService userPreferencesService;
    private bool systemPreferences;
    public UserPreferences.UserPreferences UserPreferences { get; private set; } = new();
    public DarkLightMode DarkModeToggle = DarkLightMode.System;

    public LayoutService(IUserPreferencesService userPreferencesService)
    {
        this.userPreferencesService = userPreferencesService;
    }

    public bool IsRTL { get; private set; }
    public bool IsDarkMode { get; private set; }
    public string PrimaryColor { get; set; } = "#722660";
    public string DarkPrimaryColor { get; set; } = "#0092CC";
    public string SecondaryColor { get; set; } = "#ff4081ff";
    public double BorderRadius { get; set; } = 4;
    public double DefaultFontSize { get; set; } = 1;
    public MudTheme CurrentTheme { get; private set; } = new();

    public void SetDarkMode(bool value)
    {
        UserPreferences.IsDarkMode = value;
    }

    public async Task ApplyUserPreferences(bool isDarkModeDefaultTheme)
    {
        UserPreferences = await userPreferencesService.LoadUserPreferences();

        IsDarkMode = UserPreferences.DarkLightTheme switch
        {
            DarkLightMode.Dark => true,
            DarkLightMode.Light => false,
            DarkLightMode.System => isDarkModeDefaultTheme,
            _ => IsDarkMode
        };
        IsRTL = UserPreferences.RightToLeft;
        BorderRadius = UserPreferences.BorderRadius;
        DefaultFontSize = UserPreferences.DefaultFontSize;
        CurrentTheme.LayoutProperties.DefaultBorderRadius = BorderRadius + "px";
        CurrentTheme.Typography.Default.FontSize =
            DefaultFontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Button.FontSize =
            UserPreferences.ButtonFontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Body1.FontSize =
            UserPreferences.Body1FontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Body2.FontSize =
            UserPreferences.Body2FontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Caption.FontSize =
            UserPreferences.CaptionFontSize.ToString("0.0000", CultureInfo.InvariantCulture)
            + "rem";
        CurrentTheme.Typography.Overline.FontSize =
            UserPreferences.OverlineFontSize.ToString("0.0000", CultureInfo.InvariantCulture)
            + "rem";
        CurrentTheme.Typography.Subtitle1.FontSize =
            UserPreferences.Subtitle1FontSize.ToString("0.0000", CultureInfo.InvariantCulture)
            + "rem";
        CurrentTheme.Typography.Subtitle2.FontSize =
            UserPreferences.Subtitle1FontSize.ToString("0.0000", CultureInfo.InvariantCulture)
            + "rem";
    }

    public event EventHandler? MajorUpdateOccured;

    private void OnMajorUpdateOccured()
    {
        MajorUpdateOccured?.Invoke(this, EventArgs.Empty);
    }

    public Task OnSystemPreferenceChanged(bool newValue)
    {
        systemPreferences = newValue;
        if (DarkModeToggle == DarkLightMode.System)
        {
            IsDarkMode = newValue;
            OnMajorUpdateOccured();
        }

        return Task.CompletedTask;
    }

    public async Task ToggleDarkMode()
    {
        switch (DarkModeToggle)
        {
            case DarkLightMode.System:
                DarkModeToggle = DarkLightMode.Light;
                IsDarkMode = false;
                break;
            case DarkLightMode.Light:
                DarkModeToggle = DarkLightMode.Dark;
                IsDarkMode = true;
                break;
            case DarkLightMode.Dark:
                DarkModeToggle = DarkLightMode.System;
                IsDarkMode = systemPreferences;
                break;
        }

        UserPreferences.DarkLightTheme = DarkModeToggle;
        await userPreferencesService.SaveUserPreferences(UserPreferences);
        OnMajorUpdateOccured();
    }

    public async Task ToggleRightToLeft()
    {
        IsRTL = !IsRTL;
        UserPreferences.RightToLeft = IsRTL;
        await userPreferencesService.SaveUserPreferences(UserPreferences);
        OnMajorUpdateOccured();
    }

    public async Task SetRightToLeft()
    {
        if (!IsRTL)
        {
            await ToggleRightToLeft();
        }
    }

    public async Task SetLeftToRight()
    {
        if (IsRTL)
        {
            await ToggleRightToLeft();
        }
    }

    public void SetBaseTheme(MudTheme theme)
    {
        CurrentTheme = theme;

        if (string.IsNullOrEmpty(PrimaryColor) == false)
        {
            CurrentTheme.PaletteLight.Primary = PrimaryColor;
            CurrentTheme.PaletteDark.Primary = DarkPrimaryColor;
        }

        CurrentTheme.LayoutProperties.DefaultBorderRadius = BorderRadius + "px";
        CurrentTheme.Typography.Default.FontSize =
            DefaultFontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem"; //Added
        OnMajorUpdateOccured();
    }

    public async Task SetSecondaryColor(string color)
    {
        SecondaryColor = color;
        CurrentTheme.PaletteLight.Secondary = SecondaryColor;
        CurrentTheme.PaletteDark.Secondary = SecondaryColor;
        UserPreferences.SecondaryColor = SecondaryColor;
        await userPreferencesService.SaveUserPreferences(UserPreferences);
        OnMajorUpdateOccured();
    }

    public async Task SetBorderRadius(double size)
    {
        BorderRadius = size;
        CurrentTheme.LayoutProperties.DefaultBorderRadius = BorderRadius + "px";
        UserPreferences.BorderRadius = BorderRadius;
        await userPreferencesService.SaveUserPreferences(UserPreferences);
        OnMajorUpdateOccured();
    }

    public async Task UpdateUserPreferences(UserPreferences.UserPreferences preferences)
    {
        UserPreferences = preferences;
        IsDarkMode = UserPreferences.DarkLightTheme switch
        {
            DarkLightMode.Dark => true,
            DarkLightMode.Light => false,
            DarkLightMode.System => systemPreferences = true,
            _ => IsDarkMode
        };
        IsRTL = UserPreferences.RightToLeft;
        BorderRadius = UserPreferences.BorderRadius;
        DefaultFontSize = UserPreferences.DefaultFontSize;
        CurrentTheme.PaletteLight.Primary = PrimaryColor;
        CurrentTheme.PaletteDark.Primary = DarkPrimaryColor;
        CurrentTheme.LayoutProperties.DefaultBorderRadius = BorderRadius + "px";
        CurrentTheme.Typography.Default.FontSize =
            DefaultFontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Button.FontSize =
            UserPreferences.ButtonFontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Body1.FontSize =
            UserPreferences.Body1FontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Body2.FontSize =
            UserPreferences.Body2FontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Caption.FontSize =
            UserPreferences.CaptionFontSize.ToString("0.0000", CultureInfo.InvariantCulture)
            + "rem";
        CurrentTheme.Typography.Overline.FontSize =
            UserPreferences.OverlineFontSize.ToString("0.0000", CultureInfo.InvariantCulture)
            + "rem";
        CurrentTheme.Typography.Subtitle1.FontSize =
            UserPreferences.Subtitle1FontSize.ToString("0.0000", CultureInfo.InvariantCulture)
            + "rem";
        CurrentTheme.Typography.Subtitle2.FontSize =
            UserPreferences.Subtitle1FontSize.ToString("0.0000", CultureInfo.InvariantCulture)
            + "rem";

        await userPreferencesService.SaveUserPreferences(UserPreferences);
        OnMajorUpdateOccured();
    }
}
