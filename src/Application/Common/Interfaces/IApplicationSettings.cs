namespace Cfo.Cats.Application.Common.Interfaces;

public interface IApplicationSettings
{
    string AppName { get; set; }
    string Copyright { get; set; }
    string Version { get; set; }
    string PrimaryColour { get; set; }
    string PreLoginMessage { get; set; }

    int IdleTimeOutMinutes { get; set; }
}