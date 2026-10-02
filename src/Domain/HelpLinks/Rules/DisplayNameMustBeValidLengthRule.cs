using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.HelpLinks.Rules;

public class DisplayNameMustBeValidLengthRule(string? displayName) : IBusinessRule
{
    public bool IsBroken() => displayName is not null && displayName.Length > HelpLinkConstants.DisplayNameMaximumLength;

    public string Message => $"Help Link Url display name cannot exceed {HelpLinkConstants.DisplayNameMaximumLength} characters.";
}
