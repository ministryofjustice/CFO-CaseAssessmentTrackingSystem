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

        [Description("Description")]
        public string? Description { get; set; }

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

            if(request.Description is not null)
            {
                task.Rename(request.Description);
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
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            var today = DateTime.UtcNow;

            RuleFor(x => x.ObjectiveId)
                .NotNull();

            RuleFor(x => x.TaskId)
                .NotNull();

            RuleFor(x => x.PathwayPlanId)
                .NotNull()
                .WithMessage("You must provide a Pathway Plan");
                
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("You must provide a description")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Description"));

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

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(x => x.PathwayPlanId)
                    .MustAsync(ParticipantMustNotBeArchived)
                    .WithMessage("Participant is archived");
            });
        }

        private async Task<bool> ParticipantMustNotBeArchived(Guid pathwayPlanId, CancellationToken cancellationToken)
        {
            var participantId = await (from pp in _unitOfWork.DbContext.PathwayPlans
                                       join p in _unitOfWork.DbContext.Participants on pp.ParticipantId equals p.Id
                                       where (pp.Id == pathwayPlanId
                                       && p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value)
                                       select p.Id
                                       )
                            .AsNoTracking()
                            .FirstOrDefaultAsync();

            return participantId != null;
        }
    }
}