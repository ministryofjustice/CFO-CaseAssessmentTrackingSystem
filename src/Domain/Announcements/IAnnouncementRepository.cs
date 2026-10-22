namespace Cfo.Cats.Domain.Announcements;

public interface IAnnouncementRepository
{
    Task<string> GetTypeNameAsync(string name, CancellationToken cancellationToken);
    Task AddAsync(Announcement announcement, CancellationToken cancellationToken);
    Task<Announcement> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
