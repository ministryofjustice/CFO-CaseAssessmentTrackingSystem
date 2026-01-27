using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class CloseOffLastArchivedCaseEntry(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantEnrolmentHistoryCreatedDomainEvent>
{
    public async Task Handle(ParticipantEnrolmentHistoryCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Entity.EnrolmentStatus.Name != EnrolmentStatus.ArchivedStatus.Name)
        {
            return;
        }
    
        var last = await unitOfWork.DbContext.ArchivedCases
            .Where(e => 
                    e.ParticipantId == notification.Entity.ParticipantId 
                    && e.From <= notification.Entity.From
                    && e.To == null
                    )
            .OrderByDescending(e => e.Created)
            .FirstOrDefaultAsync(cancellationToken);

        last?.SetTo(notification.Entity.From);
    }
}