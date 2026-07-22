using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Commands;

public static class AddObjective
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : ICommand<Result>
    {
        [Description("Pathway Plan Id")]
        public required Guid PathwayPlanId { get; init; }

        [Description("Description")]
        public string? Description { get; set; }

        [Description("Initiative")]
        public Guid? InitiativeId { get; set; }

        [Description("The participant's first day on the initiative")]
        public DateTime? InitiativeStartDate { get; set; }

        public class Mapping : Profile
        {
            public Mapping() =>
                CreateMap<Command, Objective>(MemberList.None)
                    .ConstructUsing(dto => Objective.Create(dto.Description!, dto.PathwayPlanId, false));
        }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var objective = mapper.Map<Objective>(request);

            var pathwayPlan = await unitOfWork.DbContext.PathwayPlans.FindAsync(request.PathwayPlanId, cancellationToken);

            if (pathwayPlan is null)
            {
                throw new NotFoundException("Cannot find pathway plan", request.PathwayPlanId);
            }

            pathwayPlan.AddObjective(objective);

            if (request.InitiativeId.HasValue)
            {
                var link = InitiativeObjective.Create(objective.Id, request.InitiativeId.Value, pathwayPlan.ParticipantId, DateOnly.FromDateTime(request.InitiativeStartDate!.Value));
                await unitOfWork.DbContext.InitiativeObjectives.AddAsync(link, cancellationToken);
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

            RuleFor(x => x.PathwayPlanId)
                .NotNull()
                .WithMessage("Pathway Plan should not be empty")
                .MustAsync(ParticipantMustNotBeArchived)
                .WithMessage("Participant is archived");

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

            RuleSet(ValidationConstants.RuleSet.Mediator, () =>
            {
                RuleFor(x => x.PathwayPlanId)
                    .MustAsync(ParticipantMustNotBeArchived)
                    .WithMessage("Participant is archived");

                RuleFor(x => x.InitiativeStartDate)
                    .MustAsync((command, startDate, token) => BeWithinInitiativeLifetime(command.InitiativeId, startDate, token))
                    .When(x => x.InitiativeId.HasValue && x.InitiativeStartDate.HasValue)
                    .WithMessage("The participant's first day on the initiative must fall within the initiative's lifetime");
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
    }   
}