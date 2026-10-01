using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.HelpLinks.Rules;

public class TabNameMustBeValidLengthRule(string? tabName) : IBusinessRule
{
    public bool IsBroken() => tabName is not null && tabName.Length > HelpLinkConstants.TabNameMaximumLength;

    public string Message => $"Help Link Tab Name cannot exceed {HelpLinkConstants.TabNameMaximumLength} characters.";
}
