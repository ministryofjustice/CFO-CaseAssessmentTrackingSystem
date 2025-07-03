using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.Features.Transfers.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Transfers.Commands;

public static class ProcessIncomingTransfer
{
    [RequestAuthorize(Policy = SecurityPolicies.UserHasAdditionalRoles)]
    public class Command : IRequest<Result>
    {
        public required IncomingTransferDto IncomingTransfer { get; set; }

        [Description("Assignee")]
        public ApplicationUserDto? Assignee { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants
                .FindAsync([request.IncomingTransfer.ParticipantId], cancellationToken);

            var transfer = await unitOfWork.DbContext.ParticipantIncomingTransferQueue
                .FindAsync([request.IncomingTransfer.Id], cancellationToken);

            if (participant is null || transfer is null)
            {
                return Result.Failure();
            }

            // Assign participant
            participant.AssignTo(request.Assignee?.Id);

            // Complete transfer
            transfer.Complete();

            return await Task.FromResult(Result.Success());
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        IUnitOfWork unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            RuleFor(c => c.IncomingTransfer)
                .NotNull();

            RuleFor(c => c.Assignee)
                .NotNull()
                .WithMessage("You must choose an assignee");

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.IncomingTransfer)
                    .MustAsync(NotBeCompleted)
                    .WithMessage("Transfer already completed");
            });
        }

        async Task<bool> NotBeCompleted(IncomingTransferDto transfer, CancellationToken cancellationToken) 
            => await unitOfWork.DbContext.ParticipantIncomingTransferQueue
                .AnyAsync(t => t.Id == transfer.Id && t.Completed == false, cancellationToken);
    }
}