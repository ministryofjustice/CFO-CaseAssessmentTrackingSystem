using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Activities.Commands
{
    public static class SubmitActivityEscalationResponse
    {
        [RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
        public class Command : IRequest<Result>
        {
            public required Guid ActivityQueueEntryId { get; set; }

            public EscalationResponse? Response { get; set; }

            public string Message { get; set; } = default!;

            public bool IsMessageExternal { get; set; }
            public UserProfile? CurrentUser { get; set; }
        }

        public enum EscalationResponse
        {
            Accept = 0,
            Return = 1,
            Comment = 2
        }

        public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
        {
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var entry = await unitOfWork.DbContext.ActivityEscalationQueue
                    .Include(pqa => pqa.Participant)
                    .FirstOrDefaultAsync(x => x.Id == request.ActivityQueueEntryId, cancellationToken: cancellationToken);

                if (entry == null)
                {
                    return Result.Failure("Cannot find activity queue item");
                }

                entry.AddNote(request.Message, request.IsMessageExternal);

                switch (request.Response)
                {
                    case EscalationResponse.Accept:
                        entry.Accept();
                        break;
                    case EscalationResponse.Return:
                        entry.Return();
                        break;
                    case EscalationResponse.Comment:
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

                When(x => x.Response is EscalationResponse.Return or EscalationResponse.Comment, () =>
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

                RuleFor(c => c.ActivityQueueEntryId)
                    .Must(Exist)
                    .WithMessage("Activity Queue item does not exist");
            }

            private bool Exist(Guid identifier) => _unitOfWork.DbContext.ActivityEscalationQueue.Any(e => e.Id == identifier);
        }

        public class C_ShouldNotBeComplete : AbstractValidator<Command>
        {
            private readonly IUnitOfWork _unitOfWork;

            public C_ShouldNotBeComplete(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;

                RuleFor(c => c.ActivityQueueEntryId)
                    .Must(BeOpen)
                    .WithMessage("Activity Queue item is already completed.");
            }

            private bool BeOpen(Guid id)
            {
                var entry = _unitOfWork.DbContext.ActivityEscalationQueue
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

                RuleFor(c => c.ActivityQueueEntryId)
                    .Must(BeAtSubmittedToAuthority)
                    .WithMessage("Activity Queue item is not at Submitted to Authority stage");
            }

            private bool BeAtSubmittedToAuthority(Guid id)
            {
                var entry = _unitOfWork.DbContext.ActivityEscalationQueue.Include(c => c.Activity)
                    .FirstOrDefault(a => a.Id == id);

                return entry != null && entry.Activity!.Status == ActivityStatus.SubmittedToAuthorityStatus;
            }
        }

        public class E_ParticipantMustHaveOwner : AbstractValidator<Command>
        {
            private readonly IUnitOfWork _unitOfWork;
            public E_ParticipantMustHaveOwner(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;

                RuleFor(c => c)
                    .Must(ParticipantMustHaveOwner)
                    .WithMessage("Participant must have an owner on approval. Please return for reassignment")
                    .When(c => c.Response is EscalationResponse.Accept);
            }

            private bool ParticipantMustHaveOwner(Command command)
            {
                var entry = _unitOfWork.DbContext.ActivityEscalationQueue.Include(c => c.Participant)
                    .First(a => a.Id == command.ActivityQueueEntryId);

                return entry.Participant?.OwnerId is not null;
            }
        }
    }
}