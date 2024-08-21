using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Queries;

public static class GetPathwayPlanByParticipantId
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<PathwayPlanDto?>
    {
        public required string ParticipantId {  get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, PathwayPlanDto?>
    {
        public async Task<PathwayPlanDto?> Handle(Query request, CancellationToken cancellationToken)
        {
            var pathwayPlan = await unitOfWork.DbContext.PathwayPlans
                .Include(p => p.ReviewHistories)
                .Where(p => p.ParticipantId == request.ParticipantId)
                .ProjectTo<PathwayPlanDto>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);

            return pathwayPlan;
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
