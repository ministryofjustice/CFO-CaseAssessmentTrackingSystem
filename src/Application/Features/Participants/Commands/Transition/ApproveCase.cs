using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Commands.Transition;

public static class ApproveCase
{
    [RequestAuthorize(Policy = PolicyNames.CanApprove)]
    public class Command : IRequest<Result>
    {
        public required string ParticipantId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants.FindAsync(request.ParticipantId);
            
            // do we have a second PQA?
            var pq2 = await unitOfWork.DbContext.EnrolmentQa2Queue.FirstOrDefaultAsync(x => x.ParticipantId == request.ParticipantId && x.IsCompleted, cancellationToken: cancellationToken);

            if (pq2 == null)
            {
                // we do not have an open PQA2 entry create one
                var entry = EnrolmentQa2QueueEntry.Create(request.ParticipantId);
                await unitOfWork.DbContext.EnrolmentQa2Queue.AddAsync(entry, cancellationToken);
            }
            else
            {
                // we have an enrolment pqa2 queue entry, so we can approve
                participant!.TransitionTo(EnrolmentStatus.ApprovedStatus);                
            }
            // ReSharper disable once MethodHasAsyncOverload
            return Result.Success();
        }
    }

    public class A_ParticipantMustExistValidator : AbstractValidator<SubmitToProviderQa.Command> 
    {
        private readonly IUnitOfWork _unitOfWork;
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
}
