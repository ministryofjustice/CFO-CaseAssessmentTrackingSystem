using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Common.Exceptions;

public class BusinessRuleValidationException(IBusinessRule brokenRule) : DomainException(brokenRule.Message)
{
    public IBusinessRule BrokenRule { get; } = brokenRule;

    public string Details { get; } = brokenRule.Message;

    public override string ToString() => $"{BrokenRule.GetType().FullName}: {BrokenRule.Message}";
}