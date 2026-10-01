namespace Cfo.Cats.Application.Common.Validators;

/// <summary>
/// Shared URL validation helpers for commands that store a URL entered by an admin
/// (e.g. help links) and need to guard against non-http(s) schemes (file://, javascript:, etc.)
/// slipping through <see cref="Uri.TryCreate(string?, UriKind, out Uri?)"/>, which otherwise
/// happily parses path-like strings such as "/some/path" as an absolute "file" URI.
/// </summary>
public static class UrlValidator
{
    public static bool IsHttpOrHttpsUrl(string? url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
