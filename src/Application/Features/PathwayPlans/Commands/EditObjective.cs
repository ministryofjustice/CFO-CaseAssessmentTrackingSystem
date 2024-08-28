using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Commands;

public static class EditObjective
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        [Description("Objective Id")]
        public required Guid ObjectiveId { get; set; }

        [Description("Pathway Plan Id")]
        public required Guid PathwayPlanId { get; set; }

        [Description("Title")]
        public required string Title { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var pathwayPlan = await unitOfWork.DbContext.PathwayPlans.FindAsync(request.PathwayPlanId)
                ?? throw new NotFoundException("Cannot find pathway plan", request.PathwayPlanId);

            var objective = pathwayPlan.Objectives.FirstOrDefault(o => o.Id == request.ObjectiveId)
                ?? throw new NotFoundException("Cannot find objective", request.ObjectiveId);

            objective.Rename(request.Title);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            var today = DateTime.UtcNow;

            RuleFor(x => x.ObjectiveId)
                .NotNull();

            RuleFor(x => x.PathwayPlanId)
                .NotNull();

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("You must provide a title")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Title"));
        }

    }
}
