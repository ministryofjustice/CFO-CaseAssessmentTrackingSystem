using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using System.Threading;

namespace Cfo.Cats.Application.Features.QualityAssurance.Commands;

public static class SubmitPqaResponse
{
    [RequestAuthorize(Policy = SecurityPolicies.Pqa)]
    public class Command : IRequest<Result>
    {
        public required Guid QueueEntryId { get; set; }
        
        public PqaResponse? Response { get; set; }

        public string Message { get; set; } = default!;
        public UserProfile? CurrentUser { get; set; }

    }

    public enum PqaResponse
    {
        Accept = 0,
        Return = 1,
        Comment = 2
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var entry = await unitOfWork.DbContext.EnrolmentPqaQueue
                .Include(pqa => pqa.Participant)
                .FirstOrDefaultAsync(x => x.Id == request.QueueEntryId, cancellationToken: cancellationToken);

            if (entry == null)
            {
                return Result.Failure("Cannot find queue item");
            }

            entry.AddNote(request.Message);

            switch (request.Response)
            {
                case PqaResponse.Accept:
                    entry.Accept();
                    break;
                case PqaResponse.Return:
                    entry.Return();
                    break;
                case PqaResponse.Comment:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Result.Success();
        }
    }

    public class A_IsValidRequest : AbstractValidator<Command>
    {
        public A_IsValidRequest()
        {
            RuleFor(x => x.Response)
                .NotNull()
                .WithMessage("You must select a response");

            RuleFor(x => x.Message)
                .MaximumLength(ValidationConstants.NotesLength);

            When(x => x.Response is PqaResponse.Return or PqaResponse.Comment, () =>
            {
                RuleFor(x => x.Message)
                    .NotEmpty()
                    .WithMessage("A message is required for this response.")
                    .MaximumLength(ValidationConstants.NotesLength)
                    .WithMessage(string.Format(ValidationConstants.NotesMessage, "Message"));
            });
        }
    }

    public class B_EntryMustExist : AbstractValidator<Command> 
    {
        private readonly IUnitOfWork _unitOfWork;

        public B_EntryMustExist(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.QueueEntryId)
                .MustAsync(MustExist)
                .WithMessage("Queue item does not exist");
        }
        private async Task<bool> MustExist(Guid identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.EnrolmentPqaQueue.AnyAsync(e => e.Id == identifier, cancellationToken);
    }

    public class C_ShouldNotBeComplete : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public C_ShouldNotBeComplete(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.QueueEntryId)
                .MustAsync(MustBeOpen)
                .WithMessage("Queue item is already completed.");
        }

        private async Task<bool> MustBeOpen(Guid id, CancellationToken cancellationToken)
        {
            var entry = await _unitOfWork.DbContext.EnrolmentPqaQueue.Include(c => c.Participant)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken: cancellationToken);

            return entry is { IsCompleted: false };
        }
    }

    public class D_ShouldNotBeAtPqaStatus : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public D_ShouldNotBeAtPqaStatus(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.QueueEntryId)
                .MustAsync(MustBeAtPqA)
                .WithMessage("Queue item is not a PQA stage");
        }

        private async Task<bool> MustBeAtPqA(Guid id, CancellationToken cancellationToken)
        {
            var entry = await _unitOfWork.DbContext.EnrolmentPqaQueue.Include(c => c.Participant)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken: cancellationToken);

            return entry != null && entry.Participant!.EnrolmentStatus == EnrolmentStatus.SubmittedToProviderStatus;
        }
    }
    public class E_OwnerShouldNotBeApprover : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;
        public E_OwnerShouldNotBeApprover(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c)
                .MustAsync(OwnerMustNotBeApprover)
                .WithMessage("This assessment is created by you hence must not be processed at PQA stage by you");
        }

        private async Task<bool> OwnerMustNotBeApprover(Command c, CancellationToken cancellationToken)
        {
            var entry = await _unitOfWork.DbContext.EnrolmentPqaQueue.Include(c => c.Participant)
                .FirstOrDefaultAsync(a => a.Id == c.QueueEntryId, cancellationToken: cancellationToken);

            return entry != null && c.CurrentUser!.UserId.Equals(entry.Participant!.OwnerId) == false;
        }
    }

    public class F_ParticipantMustHaveOwner : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;
        public F_ParticipantMustHaveOwner(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c)
                .MustAsync(ParticipantMustHaveOwner)
                .WithMessage("Participant must have an owner on approval. Please return and reassign")
                .When(c => c.Response is PqaResponse.Accept);
        }

        private async Task<bool> ParticipantMustHaveOwner(Command command, CancellationToken cancellationToken)
        {
            var entry = await _unitOfWork.DbContext.EnrolmentPqaQueue.Include(c => c.Participant)
                .FirstAsync(a => a.Id == command.QueueEntryId, cancellationToken: cancellationToken);

            return await _unitOfWork.DbContext.Participants.AnyAsync(p => p.Id == entry.ParticipantId && p.OwnerId != null, cancellationToken);
        }
    }
}