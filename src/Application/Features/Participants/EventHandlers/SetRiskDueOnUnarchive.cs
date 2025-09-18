using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class SetRiskDueOnUnarchive(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
{
    public async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if(notification.From != EnrolmentStatus.ArchivedStatus.Value)
        {
            return;
        }

        var archivedCount = await unitOfWork.DbContext.ParticipantEnrolmentHistories
                .Where(eh => eh.ParticipantId == notification.Item.Id && eh.EnrolmentStatus == EnrolmentStatus.ArchivedStatus.Value)
                .CountAsync(cancellationToken);

        var dateOfArchive = await unitOfWork.DbContext.ParticipantEnrolmentHistories
                .Where(eh => eh.ParticipantId == notification.Item.Id && eh.EnrolmentStatus == EnrolmentStatus.ArchivedStatus.Value)
                .OrderByDescending(eh => eh.Created)
                .Select(eh => eh.Created)
                .FirstOrDefaultAsync(cancellationToken);

        var isSeventhArchive = archivedCount > 0 && archivedCount % 7 == 0;
        var archivedMoreThanSixMonthsAgo = dateOfArchive.HasValue && dateOfArchive.Value < DateTime.UtcNow.AddMonths(-6);

        // Set 'Risk Due' every seventh time the participant is archived or if they were archived more than six months ago
        if (isSeventhArchive || archivedMoreThanSixMonthsAgo)
        {
            notification.Item.SetRiskDue(DateTime.UtcNow, RiskDueReason.RemovedFromArchive);
        }
    }
}
