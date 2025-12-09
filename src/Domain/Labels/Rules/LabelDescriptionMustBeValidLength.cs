using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Labels.Rules;

public class LabelDescriptionMustBeValidLength(string description) : IBusinessRule
{
    public bool IsBroken() => description.Length is < LabelConstants.DescriptionMinimumLength or > LabelConstants.DescriptionMaximumLength;
    public string Message => $"Description must be between {LabelConstants.DescriptionMinimumLength} and {LabelConstants.DescriptionMaximumLength} characters.";
}