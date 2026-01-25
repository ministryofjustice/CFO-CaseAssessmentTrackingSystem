using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Labels.Rules;

public class DescriptionCannotBeNullOrEmpty(string description) : IBusinessRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(description);

    public string Message => "Description cannot be null or empty.";
}