using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Commands;

public static class ReviewPathwayPlan
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        public required Guid PathwayPlanId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var pathwayPlan = await unitOfWork.DbContext.PathwayPlans.FirstOrDefaultAsync(x => x.Id == request.PathwayPlanId);

            if(pathwayPlan is null)
            {
                throw new NotFoundException("Cannot find pathway plan", request.PathwayPlanId);
            }

            pathwayPlan.Review();

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.PathwayPlanId)
                .NotNull();
        }

    }
}
