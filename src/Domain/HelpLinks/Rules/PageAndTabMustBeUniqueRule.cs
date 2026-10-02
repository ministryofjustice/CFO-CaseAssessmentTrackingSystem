using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.HelpLinks.Rules;

public class PageAndTabMustBeUniqueRule(
    IHelpLinkCounter helpLinkCounter,
    string pageKey,
    string? tabName,
    Guid? excludeId = null) : IBusinessRule
{
    public bool IsBroken() => helpLinkCounter.CountForPageAndTab(pageKey, tabName, excludeId) > 0;

    public string Message => "A help link already exists for this page and tab. Edit the existing entry instead.";
}
