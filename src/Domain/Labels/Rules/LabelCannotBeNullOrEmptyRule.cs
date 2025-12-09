using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Labels.Rules;

public class LabelCannotBeNullOrEmptyRule(string label) : IBusinessRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(label);

    public string Message => "Label cannot be null or empty.";
}