using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.HelpLinks.Rules;

public class UrlsCannotBeEmptyRule(IEnumerable<string> urls) : IBusinessRule
{
    public bool IsBroken() => urls.Any() == false;

    public string Message => "At least one Help Link Url must be provided.";
}
