using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.PRIs.EventHandlers;

internal class PreventCompletionOfTaskWithoutRelease(IUnitOfWork unitOfWork) : INotificationHandler<ObjectiveTaskCompletedDomainEvent>
{
    public async Task Handle(ObjectiveTaskCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Item is { IsMandatory: true, Index: 2 } && notification.Item.CompletedStatus == CompletionStatus.Done)
        {
            var pri = await unitOfWork.DbContext.PRIs
                .Where(x => x.ObjectiveId == notification.Item.ObjectiveId)
                .Select(x => new { x.ExpectedReleaseRegionId, x.Created, x.ParticipantId } )
                .FirstAsync(cancellationToken);

            // Has the participant been in the expected release region after the PRI was created?
            var hasBeenReleasedToLocation = await unitOfWork.DbContext.ParticipantLocationHistories
                .Where(x => x.ParticipantId == pri.ParticipantId)
                .Where(x => x.From >= pri.Created)
                .Where(x => x.LocationId == pri.ExpectedReleaseRegionId)
                .AnyAsync(cancellationToken);

            if (hasBeenReleasedToLocation is false)
            {
                throw new ApplicationException("Cannot accept this task as notice of the participants' release to the expected release region has not yet been received.");
            }
        }
    }
}