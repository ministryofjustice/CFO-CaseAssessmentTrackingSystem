using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Labels.Rules;

public class LabelNamesMustBeUniqueAtContractLevelRule(ILabelCounter labelCounter, string name, string? contractId) : IBusinessRule
{
    public bool IsBroken() => labelCounter.CountVisibleLabels(name, contractId) > 0;

    public string Message => "Labels must be unique at a contract level";
}