using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Commands;

public static class EditTask
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        [Description("Task Id")]
        public required Guid TaskId { get; set; }

        [Description("Objective Id")]
        public required Guid ObjectiveId { get; set; }

        [Description("Pathway Plan Id")]
        public required Guid PathwayPlanId { get; set; }

        [Description("Title")]
        public string? Title { get; set; }

        [Description("Due")]
        public DateTime? Due { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var pathwayPlan = await unitOfWork.DbContext.PathwayPlans.FindAsync(request.PathwayPlanId)
                ?? throw new NotFoundException("Cannot find pathway plan", request.PathwayPlanId);

            var objective = pathwayPlan.Objectives.FirstOrDefault(o => o.Id == request.ObjectiveId)
                ?? throw new NotFoundException("Cannot find objective", request.ObjectiveId);

            var task = objective.Tasks.FirstOrDefault(x => x.Id == request.TaskId)
                ?? throw new NotFoundException("Cannot find task", request.TaskId);

            if(request.Title is not null)
            {
                task.Rename(request.Title);
            }

            if(request.Due.HasValue)
            {
                task.Extend(request.Due.Value);
            }

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

            RuleFor(x => x.TaskId)
                .NotNull();

            RuleFor(x => x.PathwayPlanId)
                .NotNull();

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("You must provide a title")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Title"));

            RuleFor(x => x.Due)
                .Must(x => x.HasValue)
                .WithMessage("You must provide a Due date");

            When(x => x.Due.HasValue, () =>
            {
                RuleFor(x => x.Due)
                    .GreaterThanOrEqualTo(new DateTime(today.Year, today.Month, 1))
                    .WithMessage(ValidationConstants.DateMustBeInFuture)
                    .Must(x => x!.Value.Day.Equals(1))
                    .WithMessage("Due date must fall on the first day of the month");
            });
        }

    }

}
