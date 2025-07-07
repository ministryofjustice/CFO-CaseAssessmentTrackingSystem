using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Activities.Commands;

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

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(b => b.QueueEntryId)
                    .Must(MustExist)
                    .WithMessage("Activity queue item does not exist");
            });
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

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.QueueEntryId)
                    .Must(MustBeOpen)
                    .WithMessage("Activity queue item is already completed.");
            });
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

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(d => d.QueueEntryId)
                    .Must(MustBeAtPqA)
                    .WithMessage("Activity queue item is not at PQA stage");
            });
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

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(e => e)
                    .Must(OwnerMustNotBeApprover)
                    .WithMessage("This activity is created by you hence must not be processed at PQA stage by you");
            });
        }

        private bool OwnerMustNotBeApprover(Command c)
        {
            var entry = _unitOfWork.DbContext.ActivityPqaQueue.Include(c => c.Activity)
                .FirstOrDefault(a => a.Id == c.QueueEntryId);

            return entry != null && entry.Activity!.OwnerId!.Equals(c.CurrentUser!.UserId) == false;
        }
    }

    public class F_ActivityOccurredWithin3Months : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;
        public F_ActivityOccurredWithin3Months(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(f => f)
                    .Must(NotExpired)
                    .WithMessage("This activity has expired")
                    .When(f => f.Response == PqaResponse.Accept);
            });
        }

        private bool NotExpired(Command c)
        {
            if (c.Response == PqaResponse.Accept)
            {
                var entry = _unitOfWork.DbContext.ActivityPqaQueue.Include(c => c.Activity)
                    .FirstOrDefault(a => a.Id == c.QueueEntryId);

                return entry != null && DateTime.Today <= entry.Activity!.Expiry;
            }
            return false;
        }
    }

    public class G_ParticipantMustNotBeArchived : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public G_ParticipantMustNotBeArchived(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(g => g.QueueEntryId)
                    .Must(ParticipantMustNotBeArchived)
                    .WithMessage("Participant is archived");
            });
        }

        private bool ParticipantMustNotBeArchived(Guid queueEntryId)
        {
            var entry = _unitOfWork.DbContext.ActivityPqaQueue.Include(c => c.Participant)
                .FirstOrDefault(a => a.Id == queueEntryId);
            
            return entry != null && entry.Participant!.IsActive();
        }
    }
}