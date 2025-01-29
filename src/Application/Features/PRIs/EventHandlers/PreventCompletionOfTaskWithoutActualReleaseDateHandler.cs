using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.PRIs.EventHandlers;

public class PreventCompletionOfTaskWithoutActualReleaseDateHandler(IUnitOfWork unitOfWork) : INotificationHandler<ObjectiveTaskCompletedDomainEvent>
{
    public async Task Handle(ObjectiveTaskCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Item is { IsMandatory: true, Index: 2 } && notification.Item.CompletedStatus == CompletionStatus.Done)
        {
            var pri = await unitOfWork.DbContext.PRIs
                .Where(x => x.ObjectiveId == notification.Item.ObjectiveId)
                .Select(x => new
                {
                    x.Id,
                    x.ActualReleaseDate
                })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (pri is not null && pri.ActualReleaseDate is null)
            {
                throw new ApplicationException("Cannot accept this task as the actual release date is missing from the PRI");
            }
        }
    }
}