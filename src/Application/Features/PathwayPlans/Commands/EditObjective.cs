using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.PathwayPlans.Commands;

public static class EditObjective
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : ICommand<Result>
    {
        [Description("Objective Id")]
        public required Guid ObjectiveId { get; init; }

        [Description("Pathway Plan Id")]
        public required Guid PathwayPlanId { get; init; }

        [Description("Description")]
        public required string Description { get; set; }

        [Description("Initiative")]
        public Guid? InitiativeId { get; set; }

        [Description("The participant's first day on the initiative")]
        public DateTime? InitiativeStartDate { get; set; }

        [Description("Justification")]
        public string? Justification { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger) : ICommandHandler<Command, Result>
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
                    var link = InitiativeObjective.Create(objective.Id, request.InitiativeId.Value, pathwayPlan.ParticipantId, DateOnly.FromDateTime(request.InitiativeStartDate!.Value));
                    await unitOfWork.DbContext.InitiativeObjectives.AddAsync(link, cancellationToken);
                }
                else if (objective.LinkedInitiative.InitiativeId != request.InitiativeId.Value)
                {
                    // Swapping to a different initiative effectively removes the previously linked one, so
                    // it must be audited the same way an explicit unlink is — otherwise a cross-tenant
                    // initiative could be silently overwritten by picking a replacement instead of clearing
                    // the field, bypassing the audit trail entirely.
                    var previousInitiativeId = objective.LinkedInitiative.InitiativeId;

                    objective.LinkedInitiative.Update(request.InitiativeId.Value, DateOnly.FromDateTime(request.InitiativeStartDate!.Value));

                    if (string.IsNullOrWhiteSpace(request.Justification) is false)
                    {
                        await RecordUnlinkNote(pathwayPlan.ParticipantId, previousInitiativeId, objective.Description, request.Justification!, cancellationToken);
                    }
                }
                else
                {
                    objective.LinkedInitiative.Update(request.InitiativeId.Value, DateOnly.FromDateTime(request.InitiativeStartDate!.Value));
                }
            }
            else if (objective.LinkedInitiative is not null)
            {
                var unlinkedInitiativeId = objective.LinkedInitiative.InitiativeId;

                unitOfWork.DbContext.InitiativeObjectives.Remove(objective.LinkedInitiative);

                // A justification is only collected (and required) when unlinking an initiative that
                // belongs to a different tenant/region — e.g. correcting an initiative recorded against
                // the wrong location during the backdating exercise. Record it as an auditable case note
                // on the participant. Removals within the user's own tenant/region behave exactly as
                // before and don't generate a note; the row deletion itself is still captured
                // automatically in the system audit trail.
                if (string.IsNullOrWhiteSpace(request.Justification) is false)
                {
                    await RecordUnlinkNote(pathwayPlan.ParticipantId, unlinkedInitiativeId, objective.Description, request.Justification!, cancellationToken);
                }
            }

            return Result.Success();
        }

        private async Task RecordUnlinkNote(string participantId, Guid unlinkedInitiativeId, string objectiveDescription, string justification, CancellationToken cancellationToken)
        {
            var initiative = await unitOfWork.DbContext.Initiatives
                .Where(i => i.Id == unlinkedInitiativeId)
                .Select(i => new { i.Code, i.Description })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            var participant = await unitOfWork.DbContext.Participants
                .FindAsync([participantId], cancellationToken);

            var initiativeDescription = initiative is null
                ? unlinkedInitiativeId.ToString()
                : $"{initiative.Code} - {initiative.Description}";

            if (participant is null)
            {
                logger.LogWarning(
                    "Could not record audit note for initiative unlink: participant {ParticipantId} was not found. Initiative '{InitiativeDescription}' unlinked from objective '{ObjectiveDescription}'. Justification: {Justification}",
                    participantId, initiativeDescription, objectiveDescription, justification);
                return;
            }

            participant.AddNote(new Note
            {
                Message = $"Initiative '{initiativeDescription}' unlinked from objective '{objectiveDescription}'. Justification: {justification}"
            });
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        // The FluentValidation rules below are invoked both server-side (once, sequentially, via the
        // Mediator pipeline) and client-side per-field by MudForm, which can call several field
        // validators concurrently (e.g. Initiative, Justification and Start Date all validating at
        // once). All of these rules share the same scoped DbContext (via _unitOfWork), and EF Core's
        // DbContext does not support concurrent operations on the same instance — without this, two
        // rules validating at the same time throw "A second operation was started on this context
        // instance before a previous operation completed". This semaphore serializes access so the
        // rules queue up instead of racing.
        private readonly SemaphoreSlim _dbAccessSemaphore = new(1, 1);

        public Validator(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _identityService = identityService;

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

            RuleFor(x => x.InitiativeStartDate)
                .NotNull()
                .When(x => x.InitiativeId.HasValue)
                .WithMessage("You must provide the participant's first day on the initiative when linking an initiative");

            RuleFor(x => x.InitiativeStartDate)
                .LessThan(DateTime.Today.AddDays(1).Date)
                .When(x => x.InitiativeStartDate.HasValue)
                .WithMessage("The participant's first day on the initiative cannot be in the future");

            RuleFor(x => x.Justification)
                .MaximumLength(ValidationConstants.NotesLength)
                .WithMessage($"Maximum length of justification is {ValidationConstants.NotesLength}")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Justification"))
                .When(x => string.IsNullOrEmpty(x.Justification) is false);

            RuleSet(ValidationConstants.RuleSet.Mediator, () =>
            {
                RuleFor(x => x.PathwayPlanId)                    
                    .MustAsync((pathwayPlanId, token) => Serialized(() => ParticipantMustNotBeArchived(pathwayPlanId, token), token))
                    .WithMessage("Participant is archived");

                RuleFor(x => x.InitiativeId)
                    .MustAsync((command, initiativeId, token) => Serialized(() => NotHaveActivitiesWhenChangingInitiative(command.ObjectiveId, initiativeId, token), token))
                    .WithMessage("The initiative cannot be changed or removed because activities have already been recorded against this objective's tasks");

                RuleFor(x => x.Justification)
                    .MustAsync((command, justification, token) => Serialized(() => ProvideJustificationWhenUnlinkingInitiative(command.ObjectiveId, command.InitiativeId, justification, token), token))
                    .WithMessage("You must provide a justification for removing or replacing this initiative");

                RuleFor(x => x.InitiativeId)
                    .MustAsync((command, initiativeId, token) => Serialized(() => BeAuthorizedToUnlinkInitiative(command.ObjectiveId, initiativeId, token), token))
                    .WithMessage("Removing or replacing this initiative is restricted to Contract & Performance Support Managers (CMPSM) and above");

                RuleFor(x => x.InitiativeStartDate)
                    .MustAsync((command, startDate, token) => Serialized(() => NotHaveActivitiesWhenChangingStartDate(command.ObjectiveId, command.InitiativeId, startDate, token), token))
                    .When(x => x.InitiativeId.HasValue && x.InitiativeStartDate.HasValue)
                    .WithMessage("The participant's first day on the initiative cannot be changed because activities have already been recorded against this objective's tasks");

                RuleFor(x => x.InitiativeStartDate)
                    .MustAsync((command, startDate, token) => Serialized(() => BeWithinInitiativeLifetime(command.InitiativeId, startDate, token), token))
                    .When(x => x.InitiativeId.HasValue && x.InitiativeStartDate.HasValue)
                    .WithMessage("The participant's first day on the initiative must fall within the initiative's lifetime");

                RuleFor(x => x.InitiativeStartDate)
                    .MustAsync((command, startDate, token) => Serialized(() => BeOnOrBeforeInitiativeEndDate(command.ObjectiveId, startDate, token), token))
                    .When(x => x.InitiativeId.HasValue && x.InitiativeStartDate.HasValue)
                    .WithMessage("The participant's first day on the initiative must be on or before their last day on the initiative");
            });
        }

        /// <summary>
        /// Runs a single DB-touching validation rule at a time. FluentValidation rules for this command
        /// run both server-side (once, sequentially, via the Mediator pipeline) and client-side per-field
        /// via MudForm, which can invoke several field validators concurrently (e.g. Initiative,
        /// Justification and Start Date all validating together). All of these rules share the same
        /// scoped DbContext, and EF Core's DbContext does not support concurrent operations on the same
        /// instance — without this, two rules validating at once throw "A second operation was started on
        /// this context instance before a previous operation completed."
        /// </summary>
        private async Task<bool> Serialized(Func<Task<bool>> rule, CancellationToken cancellationToken)
        {
            await _dbAccessSemaphore.WaitAsync(cancellationToken);
            try
            {
                return await rule();
            }
            finally
            {
                _dbAccessSemaphore.Release();
            }
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

        private async Task<bool> NotHaveActivitiesWhenChangingStartDate(Guid objectiveId, Guid? initiativeId, DateTime? newStartDate, CancellationToken cancellationToken)
        {
            if (!initiativeId.HasValue || !newStartDate.HasValue)
            {
                return true;
            }

            var currentStartDate = await _unitOfWork.DbContext.InitiativeObjectives
                .Where(io => io.ObjectiveId == objectiveId && io.InitiativeId == initiativeId.Value)
                .Select(io => io.StartDate)
                .FirstOrDefaultAsync(cancellationToken);

            // No existing link or start date unchanged — nothing to block
            if (currentStartDate is null || currentStartDate == DateOnly.FromDateTime(newStartDate.Value))
            {
                return true;
            }

            return !await _unitOfWork.DbContext.Activities
                .AnyAsync(a => a.ObjectiveId == objectiveId, cancellationToken);
        }

        private async Task<bool> NotHaveActivitiesWhenChangingInitiative(Guid objectiveId, Guid? newInitiativeId, CancellationToken cancellationToken)
        {
            var currentInitiativeId = await _unitOfWork.DbContext.InitiativeObjectives
                .Where(io => io.ObjectiveId == objectiveId)
                .Select(io => (Guid?)io.InitiativeId)
                .FirstOrDefaultAsync(cancellationToken);

            // No existing link or no change — nothing to validate
            if (currentInitiativeId is null || currentInitiativeId == newInitiativeId)
            {
                return true;
            }

            // CMPSM+ can always change or remove an initiative, even where activities/tasks have already
            // been recorded against the objective — this is the whole point of the ability, so they can
            // correct objectives linked to the wrong initiative regardless of tenant, whether that means
            // clearing the link or swapping in the correct replacement.
            if (await UserIsCmpsmOrAbove(cancellationToken))
            {
                return true;
            }

            // Regular users may only remove/replace an initiative link within the case's own tenant/region,
            // and only when no activities exist — exactly as before.
            if (await IsInitiativeWithinCaseTenant(objectiveId, currentInitiativeId.Value, cancellationToken))
            {
                return !await _unitOfWork.DbContext.Activities
                    .AnyAsync(a => a.ObjectiveId == objectiveId, cancellationToken);
            }

            // Changing a link to an initiative outside the case's tenant/region is otherwise blocked by
            // BeAuthorizedToUnlinkInitiative (requires CMPSM+), so it's safe to allow it through here.
            return true;
        }

        private async Task<bool> ProvideJustificationWhenUnlinkingInitiative(Guid objectiveId, Guid? newInitiativeId, string? justification, CancellationToken cancellationToken)
        {
            var currentInitiativeId = await _unitOfWork.DbContext.InitiativeObjectives
                .Where(io => io.ObjectiveId == objectiveId)
                .Select(io => (Guid?)io.InitiativeId)
                .FirstOrDefaultAsync(cancellationToken);

            // Nothing linked, or the link isn't actually being removed/replaced — no justification required
            if (currentInitiativeId is null || currentInitiativeId == newInitiativeId)
            {
                return true;
            }

            // Removing/replacing an initiative within the case's own tenant/region needs no justification,
            // exactly as before.
            if (await IsInitiativeWithinCaseTenant(objectiveId, currentInitiativeId.Value, cancellationToken))
            {
                return true;
            }

            return string.IsNullOrWhiteSpace(justification) is false;
        }

        private async Task<bool> BeAuthorizedToUnlinkInitiative(Guid objectiveId, Guid? newInitiativeId, CancellationToken cancellationToken)
        {
            var currentInitiativeId = await _unitOfWork.DbContext.InitiativeObjectives
                .Where(io => io.ObjectiveId == objectiveId)
                .Select(io => (Guid?)io.InitiativeId)
                .FirstOrDefaultAsync(cancellationToken);

            // Nothing linked, or the link isn't actually being removed/replaced — no elevated permission required
            if (currentInitiativeId is null || currentInitiativeId == newInitiativeId)
            {
                return true;
            }

            // Any authorised user may remove/replace an initiative link within the case's own tenant/region
            if (await IsInitiativeWithinCaseTenant(objectiveId, currentInitiativeId.Value, cancellationToken))
            {
                return true;
            }

            return await UserIsCmpsmOrAbove(cancellationToken);
        }

        /// <summary>
        /// True when the current user holds the ManageInitiatives permission (aligns with CMPSM and above).
        /// </summary>
        private async Task<bool> UserIsCmpsmOrAbove(CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            return await _identityService.AuthorizeAsync(userId, SecurityPolicies.ManageInitiatives, cancellationToken);
        }

        /// <summary>
        /// True when the given initiative belongs to the same tenant/region as the case (participant) the
        /// objective belongs to — comparing against the case's own tenant, not the acting user's tenant.
        /// This matters because CMPSM/internal staff operate under a national, top-level tenant that is a
        /// prefix of every regional tenant, and provider/subcontractor users can sit at a deeper tenant
        /// level than the contract they deliver under; comparing against the case's tenant instead (with a
        /// bidirectional hierarchy check) correctly identifies whether the initiative is genuinely local to
        /// the case regardless of who is editing it.
        /// </summary>
        private async Task<bool> IsInitiativeWithinCaseTenant(Guid objectiveId, Guid initiativeId, CancellationToken cancellationToken)
        {
            var caseTenantId = await (
                from io in _unitOfWork.DbContext.InitiativeObjectives
                join p in _unitOfWork.DbContext.Participants on io.ParticipantId equals p.Id
                where io.ObjectiveId == objectiveId
                select p.Owner!.TenantId
            ).AsNoTracking().FirstOrDefaultAsync(cancellationToken);

            if (string.IsNullOrEmpty(caseTenantId))
            {
                return false;
            }

            var initiativeTenantId = await _unitOfWork.DbContext.Initiatives
                .Where(i => i.Id == initiativeId)
                .Select(i => i.Contract!.Tenant!.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (string.IsNullOrEmpty(initiativeTenantId))
            {
                return false;
            }

            return caseTenantId.StartsWith(initiativeTenantId, StringComparison.Ordinal)
                   || initiativeTenantId.StartsWith(caseTenantId, StringComparison.Ordinal);
        }

        private async Task<bool> BeWithinInitiativeLifetime(Guid? initiativeId, DateTime? date, CancellationToken cancellationToken)
        {
            if (!initiativeId.HasValue || !date.HasValue)
            {
                return true;
            }

            var lifetime = await _unitOfWork.DbContext.Initiatives
                .Where(i => i.Id == initiativeId.Value)
                .Select(i => new { i.Lifetime.StartDate, i.Lifetime.EndDate })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (lifetime is null)
            {
                return true;
            }

            return date.Value >= lifetime.StartDate && date.Value <= lifetime.EndDate;
        }

        private async Task<bool> BeOnOrBeforeInitiativeEndDate(Guid objectiveId, DateTime? startDate, CancellationToken cancellationToken)
        {
            if (!startDate.HasValue)
            {
                return true;
            }

            var endDate = await _unitOfWork.DbContext.InitiativeObjectives
                .Where(io => io.ObjectiveId == objectiveId)
                .Select(io => io.EndDate)
                .FirstOrDefaultAsync(cancellationToken);

            return !endDate.HasValue || startDate.Value <= endDate.Value.ToDateTime(TimeOnly.MinValue);
        }
    }
}