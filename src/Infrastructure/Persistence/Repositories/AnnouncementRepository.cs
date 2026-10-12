using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Domain.Announcements;

namespace Cfo.Cats.Infrastructure.Persistence.Repositories;

public class AnnouncementRepository(IUnitOfWork unitOfWork) : IAnnouncementRepository
{
    public async Task<string> GetTypeNameAsync(string name, CancellationToken cancellationToken)
        => await unitOfWork.DbContext.AnnouncementTypes.Where(x => x.Name == name.Trim())
            .Select(x => x.Name).SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Choose an existing announcement type.");

    public Task AddAsync(Announcement announcement, CancellationToken cancellationToken)
        => unitOfWork.DbContext.Announcements.AddAsync(announcement, cancellationToken).AsTask();

    public async Task<Announcement> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await unitOfWork.DbContext.Announcements
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken)
            ?? throw new NotFoundException("This announcement is no longer available.");
}
