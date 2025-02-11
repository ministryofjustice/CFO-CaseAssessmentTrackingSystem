using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Activities.Commands
{
    public static class SubmitActivityPqaResponse
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
                var entry = await unitOfWork.DbContext.ActivityPqaQueue
                    .Include(pqa => pqa.Participant)
                    .FirstOrDefaultAsync(x => x.Id == request.QueueEntryId, cancellationToken: cancellationToken);

                if (entry == null)
                {
                    return Result.Failure("Cannot find activity queue item");
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
                RuleFor(a => a.Response)
                    .NotNull()
                    .WithMessage("You must select a response");

                RuleFor(a => a.Message)
                    .MaximumLength(ValidationConstants.NotesLength);

                When(a => a.Response is PqaResponse.Return or PqaResponse.Comment, () =>
                {
                    RuleFor(a => a.Message)
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

                RuleFor(b => b.QueueEntryId)
                    .Must(MustExist)
                    .WithMessage("Activity queue item does not exist");
            }
            private bool MustExist(Guid identifier)
                => _unitOfWork.DbContext.ActivityPqaQueue.Any(e => e.Id == identifier);
        }

        public class C_ShouldNotBeComplete : AbstractValidator<Command>
        {
            private readonly IUnitOfWork _unitOfWork;

            public C_ShouldNotBeComplete(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;

                RuleFor(c => c.QueueEntryId)
                    .Must(MustBeOpen)
                    .WithMessage("Activity queue item is already completed.");
            }

            private bool MustBeOpen(Guid id)
            {
                var entry = _unitOfWork.DbContext.ActivityPqaQueue
                    .FirstOrDefault(a => a.Id == id);

                return entry is { IsCompleted: false };
            }
        }

        public class D_ShouldNotBeAtPqaStatus : AbstractValidator<Command>
        {
            private readonly IUnitOfWork _unitOfWork;

            public D_ShouldNotBeAtPqaStatus(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;

                RuleFor(d => d.QueueEntryId)
                    .Must(MustBeAtPqA)
                    .WithMessage("Activity queue item is not at PQA stage");
            }

            private bool MustBeAtPqA(Guid id)
            {
                var entry = _unitOfWork.DbContext.ActivityPqaQueue.Include(c => c.Activity)
                    .FirstOrDefault(a => a.Id == id);

                return entry != null && entry.Activity!.Status == ActivityStatus.SubmittedToProviderStatus;
            }
        }

        public class E_OwnerShouldNotBeApprover : AbstractValidator<Command>
        {
            private readonly IUnitOfWork _unitOfWork;
            public E_OwnerShouldNotBeApprover(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;

                RuleFor(e => e)
                    .Must(OwnerMustNotBeApprover)
                    .WithMessage("This activity is created by you hence must not be processed at PQA stage by you");
            }

            private bool OwnerMustNotBeApprover(Command c)
            {
                var entry = _unitOfWork.DbContext.ActivityPqaQueue.Include(c => c.Activity)
                    .FirstOrDefault(a => a.Id == c.QueueEntryId);

                return entry != null && c.CurrentUser!.UserId.Equals(entry.Activity!.OwnerId) == false;
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
                    .WithMessage("Participant must have an owner on approval. Please return for reassignment")
                    .When(c => c.Response is PqaResponse.Accept);
            }

            private async Task<bool> ParticipantMustHaveOwner(Command command, CancellationToken cancellationToken)
            {
                var entry = await _unitOfWork.DbContext.ActivityPqaQueue.Include(c => c.Participant)
                    .FirstAsync(a => a.Id == command.QueueEntryId, cancellationToken: cancellationToken);

                return await _unitOfWork.DbContext.Participants.AnyAsync(p => p.Id == entry.ParticipantId && p.OwnerId != null, cancellationToken);
            }
        }

        public class G_ActivityOccurredWithin3Months : AbstractValidator<Command>
        {
            private readonly IUnitOfWork _unitOfWork;
            public G_ActivityOccurredWithin3Months(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;

                RuleFor(g => g)
                    .Must(ActivityOccurredWithin3Months)
                    .WithMessage("This activity took place over 3 months ago")
                    .When(g => g.Response == PqaResponse.Accept);
            }

            private bool ActivityOccurredWithin3Months(Command c)
            {
                if (c.Response == PqaResponse.Accept)
                {
                    var entry = _unitOfWork.DbContext.ActivityPqaQueue.Include(c => c.Activity)
                        .FirstOrDefault(a => a.Id == c.QueueEntryId);

                    return entry != null && entry.Activity!.CommencedOn >= DateTime.Now.AddDays(-90);                    
                }
                return false;
            }
        }
    }
}