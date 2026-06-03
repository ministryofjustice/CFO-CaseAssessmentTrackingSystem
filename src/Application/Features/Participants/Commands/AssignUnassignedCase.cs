using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class AssignUnassignedCase
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : IRequest<Result<bool>>
    {
        public required string ParticipantId { get; set; }

        public UserProfile? CurrentUser { get; set; }

        public string? AssigneeId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants
                .FirstOrDefaultAsync(x => x.Id == request.ParticipantId, cancellationToken);

            if (participant is null)
            {
                return Result<bool>.Failure("Participant not found");
            }

            if (participant.OwnerId is not null)
            {
                return Result<bool>.Failure("Participant already has an owner");
            }

            participant.AssignTo(request.AssigneeId);

            return Result<bool>.Success(true);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.AssigneeId)
                .NotNull()
                .MinimumLength(36)
                .WithMessage("Assignee must be selected");

            RuleFor(x => x.ParticipantId)
                .NotNull()
                .Length(9)
                .WithMessage("Invalid Participant Id")
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

            RuleFor(x => x.CurrentUser)
                .NotNull()
                .WithMessage("Current user is required");

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(p => p.ParticipantId)
                    .MustAsync(ParticipantExists)
                    .WithMessage("Participant does not exist")
                    .MustAsync(ParticipantHasNoOwner)
                    .WithMessage("Participant already has an owner")
                    .MustAsync(ParticipantNotArchived)
                    .WithMessage("Participant is archived");

                RuleFor(c => c)
                    .MustAsync(ValidAssignee)
                    .WithMessage("Selected assignee is not valid or not within your tenant");

                RuleFor(c => c.ParticipantId)
                    .Must(NotHaveActiveTransfer)
                    .WithMessage("Participant has an active incoming transfer. Please process the transfer first.");
            });
        }

        private async Task<bool> ParticipantExists(string participantId, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(p => p.Id == participantId, cancellationToken);

        private async Task<bool> ParticipantHasNoOwner(string participantId, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(p => p.Id == participantId && p.OwnerId == null, cancellationToken);

        private async Task<bool> ParticipantNotArchived(string participantId, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(
                e => e.Id == participantId && e.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value, 
                cancellationToken);

        private async Task<bool> ValidAssignee(Command c, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(c.AssigneeId) || c.CurrentUser is null)
            {
                return false;
            }

            // Get the participant to determine their tenant
            var participant = await _unitOfWork.DbContext.Participants
                .Include(p => p.CurrentLocation)
                    .ThenInclude(l => l.Tenants)
                .FirstOrDefaultAsync(p => p.Id == c.ParticipantId, cancellationToken);

            if (participant is null)
            {
                return false;
            }

            // Get the participant's tenant from their current location
            var participantTenantId = participant.CurrentLocation!.Tenants
                .Select(t => t.Id)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(participantTenantId))
            {
                return false;
            }

            // Check the assignee is within the participant's tenant
            return await _unitOfWork.DbContext.Users
                .Where(x => x.TenantId!.StartsWith(participantTenantId)
                            && x.Id == c.AssigneeId
                            && x.IsActive == true)
                .AnyAsync(cancellationToken);
        }

        private bool NotHaveActiveTransfer(string participantId)
            => !_unitOfWork.DbContext.ParticipantIncomingTransferQueue
                .Any(p => p.ParticipantId == participantId && p.Completed == false);
    }
}
