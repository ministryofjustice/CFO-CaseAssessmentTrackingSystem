using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Activities.Queries;

public static class GetActivityEscalationEntryById
{
    [RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
    public class Query : IRequest<Result<ActivityQueueEntryDto>>
    {
        public Guid Id { get; init; }
        public UserProfile? CurrentUser { get; init; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<ActivityQueueEntryDto>>
    {
        public async Task<Result<ActivityQueueEntryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var entry = await unitOfWork.DbContext.ActivityEscalationQueue
                .Where(a => a.Id == request.Id && a.TenantId.StartsWith(request.CurrentUser!.TenantId!))
                .ProjectTo<ActivityQueueEntryDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (entry == null)
            {
                //question: do we return a specific error if the entry exists but is at a different tenant?
                return Result<ActivityQueueEntryDto>.Failure("Not found");
            }

            entry.Qa1CompletedBy  = await unitOfWork.DbContext.ActivityQa1Queue
                .Where(q1 => q1.ActivityId==entry.ActivityId &&
                             q1.IsCompleted)
                .OrderByDescending(q1 => q1.LastModified)
                .Select(q1 => q1.Owner!.DisplayName)
                .FirstOrDefaultAsync(cancellationToken);
            
            return entry;
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(r => r.Id)
                .NotEmpty()
                .WithMessage(string.Format(ValidationConstants.GuidMessage, "Id"));
        }
    }
}