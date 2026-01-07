using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Queries;

public static class GetEscalationEntryById
{
    [RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
    public class Query : IRequest<Result<EnrolmentQueueEntryDto>>
    {
        public Guid Id { get; set; }
        public UserProfile? CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<EnrolmentQueueEntryDto>>
    {
        public async Task<Result<EnrolmentQueueEntryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var entry = await unitOfWork.DbContext.EnrolmentEscalationQueue
                .Where(a => a.Id == request.Id && a.TenantId.StartsWith(request.CurrentUser!.TenantId!))
                .ProjectTo<EnrolmentQueueEntryDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (entry == null)
            {
                //question: do we return a specific error if the entry exists but is at a different tenant?
                return Result<EnrolmentQueueEntryDto>.Failure("Not found");
            }
            
            entry.Qa1CompletedBy = await unitOfWork.DbContext.EnrolmentQa1Queue
                .Where(q1 =>
                    q1.ParticipantId == entry.ParticipantId &&
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