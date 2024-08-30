using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public class AddOrUpdateSupervisor
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        [Description("Participant Id")]
        public required string ParticipantId { get; set; }

        public required ParticipantSupervisorDto Supervisor { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants
                .SingleOrDefaultAsync(s => s.Id == request.ParticipantId, cancellationToken);

            if(participant is null)
            {
                return Result.Failure("Participant not found");
            }

            var supervisor = mapper.Map(request.Supervisor, participant.Supervisor);

            participant.AddOrUpdateSupervisor(supervisor);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(p => p.ParticipantId)
                .NotNull()
                .Length(9)
                .WithMessage("Invalid Participant Id")
                .MustAsync(Exist)
                .WithMessage("Participant does not exist")
                .Matches(ValidationConstants.AlphaNumeric).WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

            RuleFor(p => p.Supervisor.Name)
                .MaximumLength(128)
                .Matches(ValidationConstants.NameCompliantWithDMS)
                .When(p => string.IsNullOrEmpty(p.Supervisor.Name) is false)
                .WithMessage(string.Format(ValidationConstants.NameCompliantWithDMSMessage, "Name"));

            RuleFor(p => p.Supervisor.TelephoneNumber)
                .MaximumLength(16)
                .WithMessage("Must not exceed 16 characters")
                .Matches(ValidationConstants.Numbers)
                .When(p => string.IsNullOrEmpty(p.Supervisor.TelephoneNumber) is false)
                .WithMessage(string.Format(ValidationConstants.NumbersMessage, "Telephone Number"));

            RuleFor(p => p.Supervisor.MobileNumber)
                .MaximumLength(16)
                .WithMessage("Must not exceed 16 characters")
                .Matches(ValidationConstants.Numbers)
                .When(p => string.IsNullOrEmpty(p.Supervisor.MobileNumber) is false)
                .WithMessage(string.Format(ValidationConstants.NumbersMessage, "Mobile Number"));

            RuleFor(p => p.Supervisor.EmailAddress)
                .MaximumLength(100)
                .WithMessage("Must not exceed 100 characters")
                .EmailAddress()
                .When(p => string.IsNullOrEmpty(p.Supervisor.EmailAddress) is false)
                .WithMessage("Must be a valid Email Address");

            RuleFor(p => p.Supervisor.Address)
                .MaximumLength(256)
                .WithMessage("Must not exceed 256 characters")
                .Matches(ValidationConstants.Notes)
                .When(p => string.IsNullOrEmpty(p.Supervisor.Address) is false)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Participant Id"));
        }

        private async Task<bool> Exist(string identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);
    }
}
