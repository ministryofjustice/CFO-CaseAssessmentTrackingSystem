using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.HelpLinks.Rules;

public class TitleCannotBeNullOrEmptyRule(string title) : IBusinessRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(title);

    public string Message => "Help Link Title cannot be null or empty.";
}
