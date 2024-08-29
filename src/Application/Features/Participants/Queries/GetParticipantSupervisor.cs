using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public class GetParticipantSupervisor
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<ParticipantSupervisorDto?>>
    {
        public required string ParticipantId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<ParticipantSupervisorDto?>>
    {
        public async Task<Result<ParticipantSupervisorDto?>> Handle(Query request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants
                .Include(p => p.Supervisor)
                .SingleOrDefaultAsync(s => s.Id == request.ParticipantId, cancellationToken);

            if(participant is null)
            {
                return Result<ParticipantSupervisorDto?>.Failure("Participant not found");
            }

            return mapper.Map<ParticipantSupervisorDto?>(participant.Supervisor);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .NotNull()
                .Length(9)
                .WithMessage("Invalid Participant Id")
                .MustAsync(Exist)
                .WithMessage("Participant does not exist")
                .Matches(ValidationConstants.AlphaNumeric).WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));
        }

        private async Task<bool> Exist(string identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);
    }
}
