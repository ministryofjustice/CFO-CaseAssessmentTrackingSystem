using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.HelpLinks.Rules;

public class UrlMustBeValidLengthRule(string url) : IBusinessRule
{
    public bool IsBroken() => url.Length > HelpLinkConstants.UrlMaximumLength;

    public string Message => $"Help Link Url cannot exceed {HelpLinkConstants.UrlMaximumLength} characters.";
}
