using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.HelpLinks.Rules;

public class TitleMustBeValidLengthRule(string title) : IBusinessRule
{
    public bool IsBroken() => title.Length > HelpLinkConstants.TitleMaximumLength;

    public string Message => $"Help Link Title cannot exceed {HelpLinkConstants.TitleMaximumLength} characters.";
}
