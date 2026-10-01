using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.HelpLinks.Rules;

public class PageKeyCannotBeNullOrEmptyRule(string pageKey) : IBusinessRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(pageKey);

    public string Message => "Help Link Page cannot be null or empty.";
}
