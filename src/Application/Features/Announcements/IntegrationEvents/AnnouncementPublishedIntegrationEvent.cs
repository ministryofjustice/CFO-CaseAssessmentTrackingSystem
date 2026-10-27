using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Announcements;

namespace Cfo.Cats.Application.Features.Announcements.IntegrationEvents;

public record AnnouncementPublishedIntegrationEvent(
    Guid AnnouncementId,
    Guid RevisionId,
    string Title,
    AnnouncementAudience Audience,
    DateTime OccurredOn) : IntegrationEvent(OccurredOn);
