using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.HelpLinks.Rules;

public class DescriptionMustBeValidLengthRule(string description) : IBusinessRule
{
    public bool IsBroken() => description.Length > HelpLinkConstants.DescriptionMaximumLength;

    public string Message => $"Help Link Description cannot exceed {HelpLinkConstants.DescriptionMaximumLength} characters.";
}
