using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Labels.Rules;

public class NameMustBeValidLength(string label) : IBusinessRule
{
    public bool IsBroken() => label.Length is < LabelConstants.NameMinimumLength or > LabelConstants.NameMaximumLength;
    public string Message => $"Label must be between {LabelConstants.NameMinimumLength} and {LabelConstants.NameMaximumLength} characters.";
}