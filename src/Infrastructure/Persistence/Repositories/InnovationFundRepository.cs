using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Infrastructure.Persistence.Repositories;

public class InnovationFundRepository(IUnitOfWork unitOfWork) : IInnovationFundRepository
{
    public Task AddAsync(InnovationFund fund)
        => unitOfWork.DbContext.InnovationFunds.AddAsync(fund).AsTask();

    public async Task<InnovationFund> GetByIdAsync(Guid id)
        => await unitOfWork.DbContext.InnovationFunds
               .FirstOrDefaultAsync(x => x.Id == id)
           ?? throw new NotFoundException("InnovationFund", id);
}
