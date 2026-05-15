namespace Cfo.Cats.Domain.Entities.Administration;

public interface IInitiativeRepository
{
    Task AddAsync(Initiative fund);
    Task<Initiative> GetByIdAsync(Guid id);
}
