using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.HelpLinks.Events;
using Cfo.Cats.Domain.HelpLinks.Rules;

namespace Cfo.Cats.Domain.HelpLinks;

public class HelpLink : BaseAuditableEntity<Guid>
{
    private readonly List<HelpLinkUrl> _urls = new();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private HelpLink()
    {
    }
#pragma warning restore CS8618

    private HelpLink(
        string title,
        string description,
        IEnumerable<HelpLinkUrlInput> urls,
        string pageKey,
        string? tabName,
        IHelpLinkCounter helpLinkCounter)
    {
        var normalizedTabName = string.IsNullOrWhiteSpace(tabName) ? null : tabName;

        CheckRule(new TitleCannotBeNullOrEmptyRule(title));
        CheckRule(new TitleMustBeValidLengthRule(title));
        CheckRule(new DescriptionCannotBeNullOrEmptyRule(description));
        CheckRule(new DescriptionMustBeValidLengthRule(description));
        CheckRule(new PageKeyCannotBeNullOrEmptyRule(pageKey));
        CheckRule(new PageKeyMustBeValidLengthRule(pageKey));
        CheckRule(new TabNameMustBeValidLengthRule(tabName));
        CheckRule(new PageAndTabMustBeUniqueRule(helpLinkCounter, pageKey, normalizedTabName));

        Id = Guid.CreateVersion7();
        Title = title;
        Description = description;
        PageKey = pageKey;
        TabName = normalizedTabName;

        SetUrls(urls);
    }

    public static HelpLink Create(
        string title,
        string description,
        IEnumerable<HelpLinkUrlInput> urls,
        string pageKey,
        string? tabName,
        IHelpLinkCounter helpLinkCounter)
        => new(title, description, urls, pageKey, tabName, helpLinkCounter);

    public string Title { get; private set; }

    public string Description { get; private set; }

    public IReadOnlyCollection<HelpLinkUrl> Urls => _urls.AsReadOnly();

    public string PageKey { get; private set; }

    public string? TabName { get; private set; }

    public HelpLink Edit(
        string title,
        string description,
        IEnumerable<HelpLinkUrlInput> urls,
        string pageKey,
        string? tabName,
        IHelpLinkCounter helpLinkCounter)
    {
        var normalizedTabName = string.IsNullOrWhiteSpace(tabName) ? null : tabName;

        CheckRule(new TitleCannotBeNullOrEmptyRule(title));
        CheckRule(new TitleMustBeValidLengthRule(title));
        CheckRule(new DescriptionCannotBeNullOrEmptyRule(description));
        CheckRule(new DescriptionMustBeValidLengthRule(description));
        CheckRule(new PageKeyCannotBeNullOrEmptyRule(pageKey));
        CheckRule(new PageKeyMustBeValidLengthRule(pageKey));
        CheckRule(new TabNameMustBeValidLengthRule(tabName));
        CheckRule(new PageAndTabMustBeUniqueRule(helpLinkCounter, pageKey, normalizedTabName, Id));

        Title = title;
        Description = description;
        PageKey = pageKey;
        TabName = normalizedTabName;

        SetUrls(urls);

        return this;
    }

    private void SetUrls(IEnumerable<HelpLinkUrlInput> urls)
    {
        var desired = urls
            .Where(u => string.IsNullOrWhiteSpace(u.Url) == false)
            .ToArray();

        CheckRule(new UrlsCannotBeEmptyRule(desired.Select(u => u.Url)));

        foreach (var url in desired)
        {
            CheckRule(new UrlMustBeValidLengthRule(url.Url));
            CheckRule(new DisplayNameMustBeValidLengthRule(url.DisplayName));
        }

        _urls.RemoveAll(existing => desired.Any(d => d.Url == existing.Url) == false);

        foreach (var url in desired)
        {
            var existing = _urls.FirstOrDefault(x => x.Url == url.Url);
            if (existing is null)
            {
                _urls.Add(new HelpLinkUrl(url.Url, url.DisplayName));
            }
            else
            {
                existing.Rename(url.DisplayName);
            }
        }
    }

    public void Delete() => AddDomainEvent(new HelpLinkDeletedDomainEvent(this));
}

