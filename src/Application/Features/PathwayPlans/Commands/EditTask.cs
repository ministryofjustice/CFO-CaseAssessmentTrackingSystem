using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Commands;

public static class EditTask
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : IRequest<Result>
    {
        [Description("Task Id")]
        public required Guid TaskId { get; init; }

        [Description("Objective Id")]
        public required Guid ObjectiveId { get; init; }

        [Description("Pathway Plan Id")]
        public required Guid PathwayPlanId { get; init; }

        [Description("Description")]
        public string? Description { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var pathwayPlan =
                await unitOfWork.DbContext.PathwayPlans.FindAsync(request.PathwayPlanId, cancellationToken)
                ?? throw new NotFoundException("Cannot find pathway plan", request.PathwayPlanId);

            var objective = pathwayPlan.Objectives.FirstOrDefault(o => o.Id == request.ObjectiveId)
                            ?? throw new NotFoundException("Cannot find objective", request.ObjectiveId);

            var task = objective.Tasks.FirstOrDefault(x => x.Id == request.TaskId)
                       ?? throw new NotFoundException("Cannot find task", request.TaskId);

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                return Result.Failure("You must provide a Description");
            }

            task.Rename(request.Description);

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
                .NotEmpty();

            RuleFor(x => x.TaskId)
                .NotEmpty();

            RuleFor(x => x.PathwayPlanId)
                .NotEmpty()
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
    }
}