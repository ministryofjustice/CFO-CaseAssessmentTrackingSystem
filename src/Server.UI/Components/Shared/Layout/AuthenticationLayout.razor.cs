namespace Cfo.Cats.Server.UI.Components.Shared.Layout;

public partial class AuthenticationLayout
{
    private MudTheme? _theme = new();
    [Inject] protected IApplicationSettings Settings { get; set; } = default!;

    protected string PrimaryColour => Settings.PrimaryColour;
    protected ThemeDarkColours PrimaryColourDark => Settings.PrimaryColourDark;

    protected override void OnInitialized() => _theme = Constants.Theme.ApplicationTheme(PrimaryColour, PrimaryColourDark);

}