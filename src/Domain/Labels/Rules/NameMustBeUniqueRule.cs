using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Labels.Rules;

public class NameMustBeUniqueRule(ILabelCounter labelCounter, string name) : IBusinessRule
{
    public bool IsBroken() => labelCounter.CountLabelsWithName(name) > 0;

    public string Message => "Label names must be unique";
}
