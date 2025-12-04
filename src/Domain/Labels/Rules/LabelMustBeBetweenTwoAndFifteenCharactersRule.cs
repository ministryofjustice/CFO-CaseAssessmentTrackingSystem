using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Labels.Rules;

public class LabelMustBeBetweenTwoAndFifteenCharactersRule(string label) : IBusinessRule
{
    public bool IsBroken() => label.Length is < 2 or > 15;
    public string Message => "Label must be between 2 and 15 characters.";
}