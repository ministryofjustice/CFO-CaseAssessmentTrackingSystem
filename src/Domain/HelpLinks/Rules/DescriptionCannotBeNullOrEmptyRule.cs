using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.HelpLinks.Rules;

public class DescriptionCannotBeNullOrEmptyRule(string description) : IBusinessRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(description);

    public string Message => "Help Link Description cannot be null or empty.";
}
