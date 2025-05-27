using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Commands;

/// <summary>
/// Used for getting the next entry in the QA2 list
/// </summary>
public static class GrabQa2Entry
{
    [RequestAuthorize(Policy = SecurityPolicies.Qa2)]
    public class Command : IRequest<Result<EnrolmentQueueEntryDto>>
    {
        public required UserProfile CurrentUser { get; set; }        
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result<EnrolmentQueueEntryDto>>
    {
        private static readonly SemaphoreSlim Semaphore = new(1, 1);
        
        public async Task<Result<EnrolmentQueueEntryDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            await Semaphore.WaitAsync(cancellationToken);

            try
            {
                var entry = await unitOfWork.DbContext.EnrolmentQa2Queue
                    .Include(q => q.SupportWorker)
                    .Where(x => x.OwnerId == request.CurrentUser.UserId)
                    .Where(x => x.IsCompleted == false)
                    .FirstOrDefaultAsync(cancellationToken);

                if (entry is null)
                {
                    entry = await unitOfWork.DbContext.EnrolmentQa2Queue
                        .Include(q => q.SupportWorker)
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
                    return Result<EnrolmentQueueEntryDto>.Success(mapper.Map<EnrolmentQueueEntryDto>(entry));
                }
                return Result<EnrolmentQueueEntryDto>.Failure("Nothing assignable");
            }
            finally
            {
                
                Semaphore.Release();
            }
        }
    }
    
}
