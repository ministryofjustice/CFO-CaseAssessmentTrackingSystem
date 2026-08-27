using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Commands;

public static class ReassignQaEntry
{
    [RequestAuthorize(Policy = SecurityPolicies.ServiceDeskManagement)]
    public class Command : ICommand<Result>
    {
        public required Guid QueueEntryId { get; set; }
        public required string NewUserId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var entry = await GetQueueItem(command.QueueEntryId);
            if (entry == null)
            {
                return Result.Failure("Queue entry not found");
            }

            entry.Reassign(command.NewUserId);

            return Result.Success();
        }

        private async Task<EnrolmentQueueEntry?> GetQueueItem(Guid queueEntryId)
        {
            var result =
                await unitOfWork.DbContext.EnrolmentQa1Queue
                    .Where(x => x.Id == queueEntryId)
                    .Cast<EnrolmentQueueEntry>()
                    .FirstOrDefaultAsync()
                ?? await unitOfWork.DbContext.EnrolmentQa2Queue
                    .Where(x => x.Id == queueEntryId)
                    .Cast<EnrolmentQueueEntry>()
                    .FirstOrDefaultAsync()
                ?? await unitOfWork.DbContext.EnrolmentEscalationQueue
                    .Where(x => x.Id == queueEntryId)
                    .Cast<EnrolmentQueueEntry>()
                    .FirstOrDefaultAsync();

            return result;       
        }

    }
}
