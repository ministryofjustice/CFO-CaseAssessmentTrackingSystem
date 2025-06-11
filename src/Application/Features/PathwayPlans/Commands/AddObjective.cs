using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Commands;

public static class AddObjective
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        [Description("Pathway Plan Id")]
        public required Guid PathwayPlanId { get; set; }

        [Description("Description")]
        public string? Description { get; set; }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Command, Objective>(MemberList.None)
                    .ConstructUsing(dto => Objective.Create(dto.Description!, dto.PathwayPlanId, false));
            }
        }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var objective = mapper.Map<Objective>(request);

            var pathwayPlan = await unitOfWork.DbContext.PathwayPlans.FindAsync(request.PathwayPlanId);

            if (pathwayPlan is null)
            {
                throw new NotFoundException("Cannot find pathway plan", request.PathwayPlanId);
            }

            pathwayPlan.AddObjective(objective);

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
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Description"));
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