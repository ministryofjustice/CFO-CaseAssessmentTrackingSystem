using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

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
            var risk = await unitOfWork.DbContext.Risks.FindAsync(request.RiskId);

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
                .WithMessage("Risk already completed");
        }

        private async Task<bool> NotBeCompleted(Guid riskId, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Risks.AnyAsync(r => r.Id == riskId && r.Completed == null, cancellationToken);
    }

}
