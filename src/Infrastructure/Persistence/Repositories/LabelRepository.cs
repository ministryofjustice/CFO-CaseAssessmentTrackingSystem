using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Infrastructure.Persistence.Repositories;

public class LabelRepository(IUnitOfWork unitOfWork) : ILabelRepository
{
    public Task AddAsync(Label label) 
        => unitOfWork.DbContext.Labels.AddAsync(label).AsTask();

    public Task<Label?> GetByIdAsync(LabelId labelId) 
        => unitOfWork.DbContext.Labels.FirstOrDefaultAsync(x => x.Id == labelId);

    public Task<int> CountParticipants(LabelId labelId) => throw new NotImplementedException();
}