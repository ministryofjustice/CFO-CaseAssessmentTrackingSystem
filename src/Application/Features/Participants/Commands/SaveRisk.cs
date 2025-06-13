using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class SaveRisk
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result<Guid>>
    {
        public required Guid RiskId { get; set; }
        public required RiskDto Risk { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService) : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var risk = await unitOfWork.DbContext.Risks
                .Include(x => x.Participant)
                .FirstOrDefaultAsync(x => x.Id == request.RiskId);

            if(risk is null)
            {
                return Result<Guid>.Failure("Risk not found");
            }

            risk = mapper.Map(request.Risk, risk);

            risk.Complete(currentUserService.UserId!);

            // Explicitly update the risk as entity tracking has been lost due to automapping
            unitOfWork.DbContext.Risks.Update(risk);

            return Result<Guid>.Success(risk.Id);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(r => r.RiskId)
                .NotNull()
                .MustAsync(NotBeCompleted)
                .WithMessage("Risk already completed")
                .MustAsync(ParticipantMustNotBeArchived)
                .WithMessage("Participant is archived"); ;
        }

        private async Task<bool> NotBeCompleted(Guid riskId, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Risks.AnyAsync(r => r.Id == riskId && r.Completed == null, cancellationToken);

        private async Task<bool> ParticipantMustNotBeArchived(Guid riskId, CancellationToken cancellationToken)
        {
            var participantId = await (from r in _unitOfWork.DbContext.Risks
                                 join p in _unitOfWork.DbContext.Participants on r.ParticipantId equals p.Id
                                 where (r.Id == riskId
                                 && p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value)
                                 select p.Id
                                   )
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

            return participantId != null;
        }
    }
}