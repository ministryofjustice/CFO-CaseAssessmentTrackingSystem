namespace Cfo.Cats.Infrastructure.Configurations;

public class HmppsAuthSettings
{
    public const string Key = "HmppsAuth";

    public string Instance { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string CallbackPath { get; set; } = "/signin-hmpps";
    public string AuthorizationEndpoint { get; set; } = string.Empty;
    public string TokenEndpoint { get; set; } = string.Empty;
    public string UserInformationEndpoint { get; set; } = string.Empty;
}
