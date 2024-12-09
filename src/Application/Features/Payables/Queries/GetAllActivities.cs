using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Payables.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Payables.Queries;

public static class GetAllActivities
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<Result<IEnumerable<ActivitySummaryDto>>>
    { 
        public required string ParticipantId { get; set; }
        public Guid? TaskId { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<IEnumerable<ActivitySummaryDto>>>
    {
        public async Task<Result<IEnumerable<ActivitySummaryDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await unitOfWork.DbContext.Activities
                .Where(a => a.ParticipantId == request.ParticipantId)
                .Where(a => request.TaskId == null || a.TaskId == request.TaskId)
                .ProjectTo<ActivitySummaryDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ParticipantId)
                .NotNull();

            RuleFor(x => x.ParticipantId)
                .Length(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));
        }
    }

}
