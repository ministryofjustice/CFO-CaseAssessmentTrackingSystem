using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Objectives.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Objectives.Queries;

public static class GetObjectivesByParticipantId
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<IEnumerable<ObjectiveDto>>
    {
        public required string ParticipantId {  get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, IEnumerable<ObjectiveDto>>
    {
        public async Task<IEnumerable<ObjectiveDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var objectives = await unitOfWork.DbContext.Objectives
                .Where(o => o.ParticipantId == request.ParticipantId)
                .Include(o => o.Tasks)
                .ProjectTo<ObjectiveDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken) ?? [];

            return objectives;
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ParticipantId)
                .NotNull();

            RuleFor(x => x.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));
        }
    }

}
