using Cfo.Cats.Domain.Common.Events;

namespace Cfo.Cats.Domain.Announcements;

public sealed class AnnouncementPublishedDomainEvent(Guid announcementId, AnnouncementRevision revision) : DomainEvent
{
    public Guid AnnouncementId { get; } = announcementId;
    public AnnouncementRevision Revision { get; } = revision;
}
