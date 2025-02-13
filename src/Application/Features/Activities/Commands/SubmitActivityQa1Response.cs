using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Activities.Commands
{
    public static class SubmitActivityQa1Response
    {
        [RequestAuthorize(Policy = SecurityPolicies.Qa1)]
        public class Command : IRequest<Result>
        {
            public required Guid ActivityQueueEntryId { get; set; }

            public bool? Accept { get; set; }

            public string Message { get; set; } = default!;

            public UserProfile? CurrentUser { get; set; }
        }

        public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
        {
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var entry = await unitOfWork.DbContext.ActivityQa1Queue
                    .Include(pqa => pqa.Participant)
                    .Include(ac=>ac.Activity)
                    .FirstOrDefaultAsync(x => x.Id == request.ActivityQueueEntryId, cancellationToken: cancellationToken);

                if (entry == null)
                {
                    return Result.Failure("Cannot find queue item");
                }

                entry.AddNote(request.Message, isExternal: false);

                if (request.Accept.GetValueOrDefault())
                {
                    entry.Accept();
                }
                else
                {
                    entry.Return();
                }

                return Result.Success();
            }
        }

        public class A_IsValidRequest : AbstractValidator<Command>
        {
            public A_IsValidRequest()
            {
                RuleFor(x => x.Accept)
                    .NotNull()
                    .WithMessage("You must accept or return the request");

                RuleFor(x => x.Message)
                    .MaximumLength(ValidationConstants.NotesLength);

                When(x => x.Accept is false, () => {
                    RuleFor(x => x.Message)
                        .NotEmpty()
                        .WithMessage("A message is required when returning")
                        .Matches(ValidationConstants.Notes)
                        .WithMessage(string.Format(ValidationConstants.NotesMessage, "Message"));
                });
            }
        }

        public class B_EntryMustExist : AbstractValidator<Command>
        {
            private IUnitOfWork _unitOfWork;
            public B_EntryMustExist(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;

                RuleFor(c => c.ActivityQueueEntryId)
                    .Must(Exist)
                    .WithMessage("Queue item does not exist");
            }
            private bool Exist(Guid identifier)
                => _unitOfWork.DbContext.ActivityQa1Queue.Any(e => e.Id == identifier);
        }

        public class C_ShouldNotBeComplete : AbstractValidator<Command>
        {
            private IUnitOfWork _unitOfWork;

            public C_ShouldNotBeComplete(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;

                RuleFor(c => c.ActivityQueueEntryId)
                    .Must(BeOpen)
                    .WithMessage("Queue item is already completed.");
            }

            private bool BeOpen(Guid id)
            {
                var entry = _unitOfWork.DbContext.ActivityQa1Queue
                    .FirstOrDefault(a => a.Id == id);

                return entry is { IsCompleted: false };
            }
        }

        public class D_ShouldNotBeAtPqaStatus : AbstractValidator<Command>
        {
            private IUnitOfWork _unitOfWork;

            public D_ShouldNotBeAtPqaStatus(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;

                RuleFor(c => c.ActivityQueueEntryId)
                    .Must(BeAtQa)
                    .WithMessage("Activity queue item is not a PQA stage");
            }

            private bool BeAtQa(Guid id)
            {
                var entry = _unitOfWork.DbContext.ActivityQa1Queue.Include(c => c.Activity)
                    .FirstOrDefault(a => a.Id == id);

                return entry != null && entry.Activity!.Status== ActivityStatus.SubmittedToAuthorityStatus;

            }
        }
        public class E_OwnerShouldNotBeApprover : AbstractValidator<Command>
        {
            private IUnitOfWork _unitOfWork;
            public E_OwnerShouldNotBeApprover(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;

                RuleFor(c => c)
                    .Must(OwnerMustNotBeApprover)
                    .WithMessage("This Activity is created by you hence must not be processed at QA1 stage by you");
            }

            private bool OwnerMustNotBeApprover(Command c)
            {
                var entry = _unitOfWork.DbContext.ActivityQa1Queue.Include(c => c.Activity)
                    .FirstOrDefault(a => a.Id == c.ActivityQueueEntryId);

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
                    .Must(ParticipantMustHaveOwner)
                    .WithMessage("Participant must have an owner on approval. Please return for reassignment")
                    .When(c => c.Accept is true);
            }

            private bool ParticipantMustHaveOwner(Command command)
            {
                var entry = _unitOfWork.DbContext.ActivityQa1Queue.Include(c => c.Participant)
                    .First(a => a.Id == command.ActivityQueueEntryId);

                return entry.Participant?.OwnerId is not null;
            }

        }
    }
}