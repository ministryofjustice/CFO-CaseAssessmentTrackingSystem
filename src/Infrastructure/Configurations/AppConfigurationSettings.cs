namespace Cfo.Cats.Infrastructure.Configurations;

public class AppConfigurationSettings : IApplicationSettings
{
    public const string Key = nameof(AppConfigurationSettings);

    public string Secret { get; set; } = string.Empty;

    public bool BehindSSLProxy { get; set; }

    public string ProxyIP { get; set; } = string.Empty;

    public string ApplicationUrl { get; set; } = string.Empty;

    public bool Resilience { get; set; }

    public string AppFlavor { get; set; } = "Blazor .NET 8.0";

    public string AppFlavorSubscript { get; set; } = ".NET 8";

    public string Company { get; set; } = "Creating Future Opportunities";

    public string Copyright { get; set; } = "@2024 Copyright";

    public string Version { get; set; } = "1.0.0";

    public string App { get; set; } = "Blazor";

    public string AppName { get; set; } = "CATS";
}