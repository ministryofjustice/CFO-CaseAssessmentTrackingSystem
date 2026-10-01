namespace Cfo.Cats.Domain.HelpLinks;

public class HelpLinkUrl
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private HelpLinkUrl()
    {
    }
#pragma warning restore CS8618

    internal HelpLinkUrl(string url, string? displayName)
    {
        Url = url;
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName;
    }

    public string Url { get; private set; }

    public string? DisplayName { get; private set; }

    public string Label => DisplayName ?? Url;

    internal void Rename(string? displayName) => DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName;
}
