using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Commands;

public static class EditObjective
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : IRequest<Result>
    {
        [Description("Objective Id")]
        public required Guid ObjectiveId { get; init; }

        [Description("Pathway Plan Id")]
        public required Guid PathwayPlanId { get; init; }

        [Description("Description")]
        public required string Description { get; set; }

        [Description("Initiative")]
        public Guid? InitiativeId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var pathwayPlan = await unitOfWork.DbContext.PathwayPlans.FindAsync(request.PathwayPlanId, cancellationToken)
                ?? throw new NotFoundException("Cannot find pathway plan", request.PathwayPlanId);

            var objective = pathwayPlan.Objectives.FirstOrDefault(o => o.Id == request.ObjectiveId)
                ?? throw new NotFoundException("Cannot find objective", request.ObjectiveId);

            objective.Rename(request.Description);

            if (request.InitiativeId.HasValue)
            {
                if (objective.LinkedInitiative is null)
                {
                    var link = InitiativeObjective.Create(objective.Id, request.InitiativeId.Value, pathwayPlan.ParticipantId);
                    await unitOfWork.DbContext.InitiativeObjectives.AddAsync(link, cancellationToken);
                }
                else if (objective.LinkedInitiative.InitiativeId != request.InitiativeId.Value)
                {
                    objective.LinkedInitiative.Update(request.InitiativeId.Value);
                }
            }
            else if (objective.LinkedInitiative is not null)
            {
                unitOfWork.DbContext.InitiativeObjectives.Remove(objective.LinkedInitiative);
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
                .WithMessage("You must provide a Pathway Plan");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("You must provide a description")
                .MaximumLength(2000)
                .WithMessage($"Maximum length of description is 2000")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Description"));
             
            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(x => x.PathwayPlanId)                    
                    .MustAsync(ParticipantMustNotBeArchived)
                    .WithMessage("Participant is archived");

                RuleFor(x => x.InitiativeId)
                    .MustAsync((command, initiativeId, token) => NotHaveActivitiesWhenChangingInitiative(command.ObjectiveId, initiativeId, token))
                    .WithMessage("The initiative cannot be changed or removed because activities have already been recorded against this objective's tasks");
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

        private async Task<bool> NotHaveActivitiesWhenChangingInitiative(Guid objectiveId, Guid? newInitiativeId, CancellationToken cancellationToken)
        {
            var currentInitiativeId = await _unitOfWork.DbContext.InitiativeObjectives
                .Where(io => io.ObjectiveId == objectiveId)
                .Select(io => (Guid?)io.InitiativeId)
                .FirstOrDefaultAsync(cancellationToken);

            // No existing link, or no change — nothing to validate
            if (currentInitiativeId is null || currentInitiativeId == newInitiativeId)
            {
                return true;
            }

            // Changing or removing — block if any activities exist on this objective's tasks
            return !await _unitOfWork.DbContext.Activities
                .AnyAsync(a => a.ObjectiveId == objectiveId, cancellationToken);
        }
    }
}