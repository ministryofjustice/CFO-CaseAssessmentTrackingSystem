﻿using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Activities.Commands;

public static class SubmitActivityQa2Response
{
    [RequestAuthorize(Policy = SecurityPolicies.Qa2)]
    public class Command : IRequest<Result>
    {
        public required Guid ActivityQueueEntryId { get; set; }

        public Qa2Response? Response { get; set; }

        public string Message { get; set; } = default!;

        public bool IsMessageExternal { get; set; }
        public UserProfile? CurrentUser { get; set; }
    }

    public enum Qa2Response
    {
        Accept = 0,
        Return = 1,
        Escalate = 2
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var entry = await unitOfWork.DbContext.ActivityQa2Queue
                .Include(pqa => pqa.Participant)
                .FirstOrDefaultAsync(x => x.Id == request.ActivityQueueEntryId, cancellationToken: cancellationToken);

            if (entry == null)
            {
                return Result.Failure("Cannot find Activity queue item");
            }

            entry.AddNote(request.Message, request.IsMessageExternal);

            switch (request.Response)
            {
                case Qa2Response.Accept:
                    entry.Accept();
                    break;
                case Qa2Response.Return:
                    entry.Return();
                    break;
                case Qa2Response.Escalate:
                    entry.Escalate();
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

            When(x => x.Response is Qa2Response.Return or Qa2Response.Escalate, () => {
                RuleFor(x => x.Message)
                    .NotEmpty()
                    .WithMessage("A message is required for this response")
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

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ActivityQueueEntryId)
                    .MustAsync(MustExist)
                    .WithMessage("Queue item does not exist");
            });
        }

        private async Task<bool> MustExist(Guid identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.ActivityQa2Queue.AnyAsync(e => e.Id == identifier, cancellationToken);
    }

    public class C_ShouldNotBeComplete : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public C_ShouldNotBeComplete(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ActivityQueueEntryId)
                    .MustAsync(MustBeOpen)
                    .WithMessage("Queue item is already completed.");
            });
        }

        private async Task<bool> MustBeOpen(Guid id, CancellationToken cancellationToken)
        {
            var entry = await _unitOfWork.DbContext.ActivityQa2Queue
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken: cancellationToken);

            return entry is { IsCompleted: false };
        }
    }

    public class D_ShouldBeAtSubmittedToAuthorityStatus : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public D_ShouldBeAtSubmittedToAuthorityStatus(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ActivityQueueEntryId)
                    .MustAsync(MustBeAtQa)
                    .WithMessage("Queue item is not a PQA stage");
            });
        }

        private async Task<bool> MustBeAtQa(Guid id, CancellationToken cancellationToken)
        {
            var entry = await _unitOfWork.DbContext.ActivityQa2Queue.Include(c => c.Activity)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken: cancellationToken);

            return entry != null && entry.Activity!.Status== ActivityStatus.SubmittedToAuthorityStatus;
        }
    }

    public class E_OwnerShouldNotBeApprover : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;
        public E_OwnerShouldNotBeApprover(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c)
                    .MustAsync(OwnerMustNotBeApprover)
                    .WithMessage("This Activity assessment is created by you hence must not be processed at QA2 stage by you");
            });
        }

        private async Task<bool> OwnerMustNotBeApprover(Command c, CancellationToken cancellationToken)
        {
            var entry = await _unitOfWork.DbContext.ActivityQa2Queue.Include(c => c.Activity)
                .FirstOrDefaultAsync(a => a.Id == c.ActivityQueueEntryId, cancellationToken: cancellationToken);

            return entry != null && entry.Activity!.OwnerId!.Equals(c.CurrentUser!.UserId) == false;
        }
    }
}