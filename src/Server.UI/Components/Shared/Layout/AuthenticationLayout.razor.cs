namespace Cfo.Cats.Server.UI.Components.Shared.Layout;

public partial class AuthenticationLayout
{
    private MudTheme? _theme = new();
    [Inject] protected IConfiguration Configuration { get; set; } = default!;

    protected string PrimaryColour => Configuration["PrimaryColour"] ?? Constants.Theme.DefaultPrimaryColour;

    protected override void OnInitialized()
    {
        _theme = Constants.Theme.ApplicationTheme(PrimaryColour);
    }
  
}