using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Commands;

public static class CompleteObjective
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : ICommand<Result>
    {
        [Description("Pathway Plan Id")]
        public required Guid PathwayPlanId { get; init; }

        [Description("Objective Id")]
        public required Guid ObjectiveId { get; init; }

        public CompletionStatus Reason { get; set; } = CompletionStatus.Done;

        public string? Justification { get; set; }

        [Description("The participant's last day on the initiative")]
        public DateTime? InitiativeEndDate { get; set; }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var pathwayPlan = await unitOfWork.DbContext.PathwayPlans.FindAsync(request.PathwayPlanId, cancellationToken)
                ?? throw new NotFoundException("Cannot find pathway plan", request.PathwayPlanId);

            var objective = pathwayPlan.Objectives.FirstOrDefault(o => o.Id == request.ObjectiveId)
                ?? throw new NotFoundException("Cannot find objective", request.ObjectiveId);

            objective.Complete(request.Reason, currentUserService.UserId!, request.Justification);

            if (request.InitiativeEndDate.HasValue && objective.LinkedInitiative is not null)
            {
                objective.LinkedInitiative.Close(DateOnly.FromDateTime(request.InitiativeEndDate.Value));
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

            RuleFor(x => x.ObjectiveId)
                .NotNull();

            RuleFor(x => x.PathwayPlanId)
                .NotNull()
                .WithMessage("You must provide a Pathway Plan")
                .MustAsync(ParticipantMustNotBeArchived)
                .WithMessage("Participant is archived");

            RuleFor(x => x.Justification)
                .NotEmpty()
                .When(c => c.Reason.RequiresJustification)
                .WithMessage("You must provide a justification for the selected reason")
                .MaximumLength(ValidationConstants.NotesLength)
                .WithMessage($"Maximum length of justification is {ValidationConstants.NotesLength}")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Justification"));
            
            RuleSet(ValidationConstants.RuleSet.Mediator, () =>
            {
                RuleFor(x => x.PathwayPlanId)
                    .MustAsync(ParticipantMustNotBeArchived)
                    .WithMessage("Participant is archived");

                RuleFor(x => x.InitiativeEndDate)
                    .MustAsync(async (command, endDate, token) =>
                    {
                        var hasInitiative = await _unitOfWork.DbContext.InitiativeObjectives
                            .AnyAsync(io => io.ObjectiveId == command.ObjectiveId, token);
                        return !hasInitiative || endDate.HasValue;
                    })
                    .WithMessage("You must provide the participant's last day on the initiative when the objective has a linked initiative");

                RuleFor(x => x.InitiativeEndDate)
                    .MustAsync((command, endDate, token) => BeWithinInitiativeLifetime(command.ObjectiveId, endDate, token))
                    .When(x => x.InitiativeEndDate.HasValue)
                    .WithMessage("The participant's last day on the initiative must fall within the initiative's lifetime");

                RuleFor(x => x.InitiativeEndDate)
                    .MustAsync((command, endDate, token) => BeOnOrAfterInitiativeStartDate(command.ObjectiveId, endDate, token))
                    .When(x => x.InitiativeEndDate.HasValue)
                    .WithMessage("The participant's last day on the initiative must be on or after their first day on the initiative");
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
                            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            return participantId != null;
        }

        private async Task<bool> BeWithinInitiativeLifetime(Guid objectiveId, DateTime? endDate, CancellationToken cancellationToken)
        {
            if (!endDate.HasValue)
            {
                return true;
            }

            var lifetime = await (
                from io in _unitOfWork.DbContext.InitiativeObjectives
                join i in _unitOfWork.DbContext.Initiatives on io.InitiativeId equals i.Id
                where io.ObjectiveId == objectiveId
                select new { i.Lifetime.StartDate, i.Lifetime.EndDate }
            ).AsNoTracking().FirstOrDefaultAsync(cancellationToken);

            if (lifetime is null)
            {
                return true;
            }

            return endDate.Value >= lifetime.StartDate && endDate.Value <= lifetime.EndDate;
        }

        private async Task<bool> BeOnOrAfterInitiativeStartDate(Guid objectiveId, DateTime? endDate, CancellationToken cancellationToken)
        {
            if (!endDate.HasValue)
            {
                return true;
            }

            var startDate = await _unitOfWork.DbContext.InitiativeObjectives
                .Where(io => io.ObjectiveId == objectiveId)
                .Select(io => io.StartDate)
                .FirstOrDefaultAsync(cancellationToken);

            return endDate.Value >= startDate.ToDateTime(TimeOnly.MinValue);
        }
    }
}