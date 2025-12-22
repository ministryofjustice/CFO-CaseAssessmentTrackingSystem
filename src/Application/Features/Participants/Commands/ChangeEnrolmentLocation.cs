using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class ChangeEnrolmentLocation
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        public string? ParticipantId { get; set; }
        public int? NewLocationId { get; set; }

        public string? JustificationReason { get; set; }

        public UserProfile? CurrentUser { get; set; } 
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext
                .Participants
                .FirstAsync(x => x.Id == request.ParticipantId, cancellationToken);

            participant.SetEnrolmentLocation(request.NewLocationId.GetValueOrDefault(), request.JustificationReason!);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .NotNull()
                .MaximumLength(9)
                .MinimumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));                

            RuleFor(c => c.NewLocationId)
                .NotNull()
                .WithMessage("New location must be provided")
                .GreaterThan(0);

            RuleFor(c => c.CurrentUser)
                .NotNull();

            RuleFor(c => c.JustificationReason)
                .NotNull()
                .NotEmpty()
                .MaximumLength(ValidationConstants.NotesLength)
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Justification Reason"));

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ParticipantId!)
                    .MustAsync(Exist)
                    .WithMessage("Participant does not exist")
                    .MustAsync(MustNotBeArchived)
                    .WithMessage("Participant is archived")
                    .MustAsync(BeChangeable)
                    .WithMessage("Participant must be changeable");
            });
        }

        private async Task<bool> Exist(string participantId, CancellationToken cancellationToken)
        {
            var participant = await _unitOfWork.DbContext
                .Participants
                .IgnoreAutoIncludes()
                .FirstOrDefaultAsync(x => x.Id == participantId, cancellationToken);

            return participant != null;
        }

        private async Task<bool> MustNotBeArchived(string participantId, CancellationToken cancellationToken)
         => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId && e.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value, cancellationToken);

        private async Task<bool> BeChangeable(string participantId, CancellationToken cancellationToken)
        {
            var participant = await _unitOfWork.DbContext
                .Participants
                .IgnoreAutoIncludes()
                .FirstAsync(x => x.Id == participantId, cancellationToken);
            
            return participant.EnrolmentStatus!.AllowEnrolmentLocationChange() && participant.ConsentStatus!.AllowEnrolmentLocationChange();
        }
    }
}