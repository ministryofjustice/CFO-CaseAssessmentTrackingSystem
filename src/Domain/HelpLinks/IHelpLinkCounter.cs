namespace Cfo.Cats.Domain.HelpLinks;

public interface IHelpLinkCounter
{
    int CountForPageAndTab(string pageKey, string? tabName, Guid? excludeId = null);
}
