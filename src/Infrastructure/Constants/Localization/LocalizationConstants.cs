namespace Cfo.Cats.Infrastructure.Constants.Localization;

public static class LocalizationConstants
{
    public const string ResourcesPath = "Resources";

    public static readonly LanguageCode[] SupportedLanguages =
    {
        new() { Code = "en-gb", DisplayName = "English (United Kingdom)" },
        new() { Code = "cy-gb", DisplayName = "Welsh" }
    };
}

public class LanguageCode
{
    public string DisplayName { get; set; } = "en-gb";
    public string Code { get; set; } = "English (United Kingdom)";
}
