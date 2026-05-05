using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Infrastructure.Persistence.Repositories;

public class InitiativeRepository(IUnitOfWork unitOfWork) : IInitiativeRepository
{
    public Task AddAsync(Initiative fund)
        => unitOfWork.DbContext.Initiatives.AddAsync(fund).AsTask();

    public async Task<Initiative> GetByIdAsync(Guid id)
        => await unitOfWork.DbContext.Initiatives
               .FirstOrDefaultAsync(x => x.Id == id)
           ?? throw new NotFoundException("Initiative", id);
}
