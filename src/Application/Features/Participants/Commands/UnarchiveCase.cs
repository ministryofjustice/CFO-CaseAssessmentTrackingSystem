using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class UnarchiveCase
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : IRequest<Result>
    {
        public required string ParticipantId { get; set; }
        [Description("Reason for Unarchive")] public UnarchiveReason UnarchiveReason { get; set; } = UnarchiveReason.CaseloadManageable;
        [Description("Additional Information")] public string? AdditionalInformation { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants
                .SingleAsync(p => p.Id == request.ParticipantId, cancellationToken);

            // Get previous status
            var previousStatus = await unitOfWork.DbContext.ParticipantEnrolmentHistories
                    .Where(eh => eh.ParticipantId == request.ParticipantId)
                    .OrderByDescending(eh => eh.Created)
                    .Skip(1)
                    .Select(eh => eh.EnrolmentStatus)
                    .FirstOrDefaultAsync(cancellationToken);

            if (previousStatus == null)
            {
                throw new Exception("The participant does not have enough history to unarchive.");
            }

            participant!.TransitionTo(previousStatus, request.UnarchiveReason.Name, request.AdditionalInformation);

            // ReSharper disable once MethodHasAsyncOverload
            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(IUnitOfWork unitOfWork)
        {
            RuleSet(ValidationConstants.RuleSet.Default, () =>
            {
                RuleFor(c => c.AdditionalInformation)
                    .NotEmpty()
                    .When(c => c.UnarchiveReason.RequiresFurtherInformation)
                    .WithMessage("You must provide additional information for the selected unarchive reason")
                    .Matches(ValidationConstants.Notes)
                    .WithMessage(string.Format(ValidationConstants.NotesMessage, "Additional Information"));
            });

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c)
                    .MustAsync(async(_, command, context, canc) =>
                    {
                        var participant = await (from p in unitOfWork.DbContext.Participants
                                          where p.Id == command.ParticipantId
                                          select new { p.Id, p.EnrolmentStatus, p.DeactivatedInFeed })
                                          .FirstOrDefaultAsync(canc);

                        var reason = participant switch
                        {
                            null => "participant does not exist",
                            { EnrolmentStatus: var status } when status != EnrolmentStatus.ArchivedStatus => "participant is not archived",
                            { DeactivatedInFeed: var date } when date < DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-30) => "post license case closure period has elapsed",
                            _ => null
                        };

                        context.MessageFormatter.AppendArgument("Reason", reason);

                        return reason is null;
                    })
                    .WithMessage("Cannot unarchive: {Reason}");
            });
        }
    }
}