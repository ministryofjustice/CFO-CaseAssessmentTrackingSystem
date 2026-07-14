namespace Cfo.Cats.Infrastructure.Configurations;

public class AppConfigurationSettings : IApplicationSettings
{
    public const string Key = nameof(AppConfigurationSettings);

    public required string AppName { get; set; }
    public required string Copyright { get; set; }
    public required string Version { get; set; }
    public required string PrimaryColour { get; set; }
    public required string PreLoginMessage { get; set; }
    public int IdleTimeOutMinutes { get; set; }
}