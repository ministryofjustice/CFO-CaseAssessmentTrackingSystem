using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Domain.HelpLinks;

namespace Cfo.Cats.Infrastructure.Persistence.Repositories;

public class HelpLinkRepository(IUnitOfWork unitOfWork) : IHelpLinkRepository
{
    public Task AddAsync(HelpLink helpLink)
        => unitOfWork.DbContext.HelpLinks.AddAsync(helpLink).AsTask();

    public async Task<HelpLink> GetByIdAsync(Guid id) =>
        await unitOfWork.DbContext
            .HelpLinks
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException("HelpLink", id);

    public async Task<IReadOnlyList<HelpLink>> GetAllAsync() =>
        await unitOfWork.DbContext.HelpLinks.ToListAsync();
}
