using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Commands;

public static class ReviewPathwayPlan
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        public required Guid PathwayPlanId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var pathwayPlan = await unitOfWork.DbContext.PathwayPlans.FirstOrDefaultAsync(x => x.Id == request.PathwayPlanId);

            if(pathwayPlan is null)
            {
                throw new NotFoundException("Cannot find pathway plan", request.PathwayPlanId);
            }

            pathwayPlan.Review();

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
                .WithMessage("You must provide a Pathway Plan")
                .MustAsync(ParticipantMustNotBeArchived)
                .WithMessage("Participant is archived"); 
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

    public class A_ParticipantMustNotBeArchived : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public A_ParticipantMustNotBeArchived(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.PathwayPlanId)
                .Must(ParticipantMustNotBeArchived)
                .WithMessage("Participant is archived");
        }

        private bool ParticipantMustNotBeArchived(Guid pathwayPlanId)
        {
            var participantId = (from pp in _unitOfWork.DbContext.PathwayPlans
                                 join p in _unitOfWork.DbContext.Participants on pp.ParticipantId equals p.Id
                                 where (pp.Id == pathwayPlanId
                                 && p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value)
                                 select p.Id
                                   )
                        .AsNoTracking()
                        .FirstOrDefault();

            return participantId != null;
        }
    }
}