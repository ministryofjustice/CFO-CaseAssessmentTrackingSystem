namespace Cfo.Cats.Domain.HelpLinks;

public interface IHelpLinkRepository
{
    Task AddAsync(HelpLink helpLink);
    Task<HelpLink> GetByIdAsync(Guid id);
    Task<IReadOnlyList<HelpLink>> GetAllAsync();
}
