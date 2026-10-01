using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.HelpLinks.Rules;

public class PageKeyMustBeValidLengthRule(string pageKey) : IBusinessRule
{
    public bool IsBroken() => pageKey.Length > HelpLinkConstants.PageKeyMaximumLength;

    public string Message => $"Help Link Page cannot exceed {HelpLinkConstants.PageKeyMaximumLength} characters.";
}
