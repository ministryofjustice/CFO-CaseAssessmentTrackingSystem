using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Infrastructure.Persistence.Repositories;

public class LabelRepository(IUnitOfWork unitOfWork) : ILabelRepository
{
    public Task AddAsync(Label label) 
        => unitOfWork.DbContext.Labels.AddAsync(label).AsTask();

    public async Task<Label> GetByIdAsync(LabelId labelId) => 
        await unitOfWork.DbContext
            .Labels
            .FirstOrDefaultAsync(x => x.Id == labelId)
            ?? throw new NotFoundException("Label", labelId);

    public Task<int> CountParticipants(LabelId labelId) => throw new NotImplementedException();
}