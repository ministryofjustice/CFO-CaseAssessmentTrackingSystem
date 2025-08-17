using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Activities.Commands;

/// <summary>
/// Used for getting the next entry in the QA2 list
/// </summary>
public static class GrabActivityQa2Entry
{
    [RequestAuthorize(Policy = SecurityPolicies.Qa2)]
    public class Command : IRequest<Result<ActivityQueueEntryDto>>
    {
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result<ActivityQueueEntryDto>>
    {
        private static readonly SemaphoreSlim Semaphore = new(1, 1);

        public async Task<Result<ActivityQueueEntryDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            await Semaphore.WaitAsync(cancellationToken);

            try
            {
                var entry = await unitOfWork.DbContext.ActivityQa2Queue
                    .Where(x => x.OwnerId == request.CurrentUser.UserId)
                    .Where(x => x.IsCompleted == false)
                    .FirstOrDefaultAsync(cancellationToken);

                if (entry is null)
                {
                    entry = await unitOfWork.DbContext.ActivityQa2Queue
                        .Where(x => x.IsCompleted == false && x.OwnerId == null && x.CreatedBy != request.CurrentUser.UserId)
                        .OrderBy(x => x.Created)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (entry is not null)
                    {
                        entry.OwnerId = request.CurrentUser.UserId;
                        await unitOfWork.CommitTransactionAsync();
                    }
                }

                if (entry is not null)
                {
                    return Result<ActivityQueueEntryDto>.Success(mapper.Map<ActivityQueueEntryDto>(entry));
                }
                return Result<ActivityQueueEntryDto>.Failure("Nothing assignable");
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}