using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Commands;

public class CompleteTask
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

        [Description("Reason")]
        public CompletionStatus Reason { get; set; } = CompletionStatus.Done;

        [Description("Justification")]
        public string Justification { get; set; } = string.Empty;
    }

    public class Handler(
        IUnitOfWork unitOfWork, 
        ICurrentUserService currentUserService) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var pathwayPlan = await unitOfWork.DbContext.PathwayPlans.FindAsync(request.PathwayPlanId)
                ?? throw new NotFoundException("Cannot find pathway plan", request.PathwayPlanId);

            var objective = pathwayPlan.Objectives.FirstOrDefault(o => o.Id == request.ObjectiveId)
                ?? throw new NotFoundException("Cannot find objective", request.ObjectiveId);

            var task = objective.Tasks.FirstOrDefault(x => x.Id == request.TaskId)
                ?? throw new NotFoundException("Cannot find task", request.TaskId);

            task.Complete(request.Reason, currentUserService.UserId!, request.Justification);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.TaskId)
                .NotNull();

            RuleFor(x => x.ObjectiveId)
                .NotNull();

            RuleFor(x => x.PathwayPlanId)
                .NotNull()
                .WithMessage("You must provide a Pathway Plan")
                .MustAsync(ParticipantMustNotBeArchived)
                .WithMessage("Participant is archived");

            When(x => x.Reason.RequiresJustification, () =>
            {
                RuleFor(x => x.Justification)
                    .NotEmpty()
                    .WithMessage("Justification is required for the selected reason");
            });

            RuleFor(x => x.Justification)
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Justification"));
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