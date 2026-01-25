using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Labels.Rules;

public class NameCannotBeNullOrEmptyRule(string name) : IBusinessRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(name);

    public string Message => "Label Name cannot be null or empty.";
}