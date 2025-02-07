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
                    .WithMessage("Activity queue item does not exist");
            }
            private async Task<bool> MustExist(Guid identifier, CancellationToken cancellationToken)
                => await _unitOfWork.DbContext.ActivityPqaQueue.AnyAsync(e => e.Id == identifier, cancellationToken);
        }

        public class C_ShouldNotBeComplete : AbstractValidator<Command>
        {
            private readonly IUnitOfWork _unitOfWork;

            public C_ShouldNotBeComplete(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;

                RuleFor(c => c.QueueEntryId)
                    .MustAsync(MustBeOpen)
                    .WithMessage("Activity queue item is already completed.");
            }

            private async Task<bool> MustBeOpen(Guid id, CancellationToken cancellationToken)
            {
                var entry = await _unitOfWork.DbContext.ActivityPqaQueue
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
                    .WithMessage("Activity queue item is not at PQA stage");
            }

            private async Task<bool> MustBeAtPqA(Guid id, CancellationToken cancellationToken)
            {
                var entry = await _unitOfWork.DbContext.ActivityPqaQueue.Include(c => c.Activity)
                    .FirstOrDefaultAsync(a => a.Id == id, cancellationToken: cancellationToken);

                return entry != null && entry.Activity!.Status == ActivityStatus.SubmittedToProviderStatus;
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
                    .WithMessage("This activity is created by you hence must not be processed at PQA stage by you");
            }

            private async Task<bool> OwnerMustNotBeApprover(Command c, CancellationToken cancellationToken)
            {
                var entry = await _unitOfWork.DbContext.ActivityPqaQueue.Include(c => c.Activity)
                    .FirstOrDefaultAsync(a => a.Id == c.QueueEntryId, cancellationToken: cancellationToken);

                return entry != null && c.CurrentUser!.UserId.Equals(entry.Activity!.OwnerId) == false;
            }
        }
    }
}
