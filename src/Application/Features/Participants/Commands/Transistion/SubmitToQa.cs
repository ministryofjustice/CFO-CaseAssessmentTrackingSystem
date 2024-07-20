using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Commands.Transistion;

public static class SubmitToQa
{
    [RequestAuthorize(Policy = PolicyNames.CanSubmitToQA)]
    public class Command : IRequest<Result>
    {
        public required string ParticipantId { get; set; }

    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants.FindAsync(request.ParticipantId);
            participant!.TransitionTo(EnrolmentStatus.SubmittedToAuthorityStatus);
            // ReSharper disable once MethodHasAsyncOverload
            return Result.Success();
        }
    }

    public class A_ParticipantMustExistValidator : AbstractValidator<Command> 
    {
        private IUnitOfWork _unitOfWork;
        public A_ParticipantMustExistValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .NotNull()
                .MinimumLength(9)
                .MaximumLength(9)
                .WithMessage("Invalid Participant Id")
                .MustAsync(MustExist)
                .WithMessage("Participant does not exist");
        }
        private async Task<bool> MustExist(string identifier, CancellationToken cancellationToken)
                => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);
    }

    public class B_ParticipantAssessmentShouldBeSubmittedToPqa : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public B_ParticipantAssessmentShouldBeSubmittedToPqa(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .MustAsync(MustBeAtPQAStage)
                .WithMessage("Assessment has not been submitted and scored.");
        }

        private async Task<bool> MustBeAtPQAStage(string identifier, CancellationToken cancellationToken)
        {
            var participant = await _unitOfWork.DbContext.Participants
                .FindAsync(identifier);

            return participant != null && participant.EnrolmentStatus == EnrolmentStatus.SubmittedToProviderStatus;

        }
    }

    
}
