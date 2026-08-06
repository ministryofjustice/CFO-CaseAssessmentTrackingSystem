namespace Cfo.Cats.Application.Common.Interfaces;

public interface IApplicationSettings
{
    string AppName { get; set; }
    string Copyright { get; set; }
    string Version { get; set; }
    string PrimaryColour { get; set; }
    ThemeDarkColours PrimaryColourDark { get; set; }
    string PreLoginMessage { get; set; }

    int IdleTimeOutMinutes { get; set; }
}

public class ThemeDarkColours
{
    public required string Primary { get; set; }
    public required string TableLines { get; set; }
    public required string AppbarBackground { get; set; }
}