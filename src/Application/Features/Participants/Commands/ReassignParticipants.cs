using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Commands
{
    public static class ReassignParticipants
    {
        [RequestAuthorize(Policy = SecurityPolicies.UserHasAdditionalRoles)]
        public class Command : IRequest<Result<bool>>
        {
            public string[] ParticipantIdsToReassign { get; set; } = [];

            public UserProfile? CurrentUser { get; set; }

            public string? AssigneeId { get; set; }
        }

        public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<bool>>
        {
            public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
            {
                foreach (var participantId in request.ParticipantIdsToReassign)
                {
                    var participant = await unitOfWork.DbContext.Participants
                        .FirstOrDefaultAsync(x => x.Id == participantId);

                    if (participant is not null)
                    {
                        participant.AssignTo(request.AssigneeId);
                    }
                    else
                    {
                        return Result<bool>.Failure("Participant not found");
                    }
                }

                return Result<bool>.Success(true);
            }
        }

        public class A_ : AbstractValidator<Command>
        {
            public A_()
            {
                RuleFor(x => x.AssigneeId)
                    .NotNull()
                    .MinimumLength(36);

                RuleFor(x => x.ParticipantIdsToReassign)
                    .NotEmpty();

                RuleFor(x => x.CurrentUser)
                    .NotNull();
            }
        }

        public class A_ParticipantExists : AbstractValidator<Command>
        {
            private readonly IUnitOfWork _unitOfWork;

            public A_ParticipantExists(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;

                // Validate each ParticipantId in the list
                RuleForEach(p => p.ParticipantIdsToReassign)
                   .NotNull()
                   .Length(9)
                   .WithMessage("Invalid Participant Id")
                   .MustAsync(Exist)
                   .WithMessage("Participant does not exist")
                   .Matches(ValidationConstants.AlphaNumeric)
                   .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"))
                   .MustAsync(MustNotBeArchived)
                   .WithMessage("Participant is archived"); ;
            }

            // Check if the participant exists in the database
            private async Task<bool> Exist(string participantId, CancellationToken cancellationToken)
                => await _unitOfWork.DbContext.Participants.AnyAsync(p => p.Id == participantId, cancellationToken);

            private async Task<bool> MustNotBeArchived(string participantId, CancellationToken cancellationToken)
                => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId && e.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value, cancellationToken);
        }

        public class B_AssignerHasAccessToAssignee : AbstractValidator<Command>
        {
            private IUnitOfWork _unitOfWork;
            public B_AssignerHasAccessToAssignee(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;

                RuleFor(c => c)
                    .MustAsync(ValidAssignee)
                    .WithMessage("This assessment is created by you hence must not be processed at PQA stage by you");
            }

            private async Task<bool> ValidAssignee(Command c, CancellationToken cancellationToken)
            {
                return await _unitOfWork.DbContext.Users.Where(x => x.TenantId!.StartsWith(c.CurrentUser!.TenantId!)
                                                                && x.Id == c.CurrentUser.UserId
                                                                && x.IsActive == true
                                                                ).AnyAsync(cancellationToken);
            }
        }

        public class C_ParticipantMustNotHaveAnOpenTransfer : AbstractValidator<Command>
        {
            private IUnitOfWork _unitOfWork;
            public C_ParticipantMustNotHaveAnOpenTransfer(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;

                RuleForEach(c => c.ParticipantIdsToReassign)
                    .Must(NotHaveAnOpenTransfer)
                    .WithMessage((command, participantId) => $"Participant {participantId} has an active transfer, you must first complete the transfer before reassigning.");
            }

            private bool NotHaveAnOpenTransfer(string participantId)
            {
                return _unitOfWork.DbContext.ParticipantIncomingTransferQueue
                    .Any(p => p.ParticipantId == participantId && p.Completed == false) is false;
            }
        }
    }
}