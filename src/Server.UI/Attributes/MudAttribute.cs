using MudBlazor;

namespace WebApp.Attributes;

public class MudAttribute : Attribute
{
    public string DisplayName { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public Color Color { get; set; } = Color.Primary;
}
