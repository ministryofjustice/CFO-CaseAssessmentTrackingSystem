using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Transfers.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Application.Features.Transfers.Commands;

public static class DismissIncomingTransfer
{
    [RequestAuthorize(Policy = SecurityPolicies.UserHasAdditionalRoles)]
    public class Command : IRequest<Result>
    {
        public required IncomingTransferDto IncomingTransfer { get; set; }
    }

    private class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
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

            transfer.Dismiss(participant.EnrolmentStatus!);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            RuleFor(c => c.IncomingTransfer)
                .NotNull();

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.IncomingTransfer)
                    .MustAsync(NotBeCompleted)
                    .WithMessage("Transfer is already completed")
                    .MustAsync(BelongToArchivedParticipant)
                    .WithMessage("Transfer can only be dismissed when the participant is in the Archived status");
            });
        }

        private async Task<bool> NotBeCompleted(IncomingTransferDto transfer, CancellationToken cancellationToken)
            => await unitOfWork.DbContext.ParticipantIncomingTransferQueue
                .AnyAsync(t => t.Id == transfer.Id && t.Completed == false, cancellationToken);

        private async Task<bool> BelongToArchivedParticipant(IncomingTransferDto transfer, CancellationToken cancellationToken)
            => await unitOfWork.DbContext.Participants
                .AnyAsync(p => p.Id == transfer.ParticipantId && p.EnrolmentStatus == EnrolmentStatus.ArchivedStatus.Value, cancellationToken);
    }
}
