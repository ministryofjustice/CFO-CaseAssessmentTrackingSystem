namespace Cfo.Cats.Domain.Entities.Administration;

public interface IInnovationFundRepository
{
    Task AddAsync(InnovationFund fund);
    Task<InnovationFund> GetByIdAsync(Guid id);
}
