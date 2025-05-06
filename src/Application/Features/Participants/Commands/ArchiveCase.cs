using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class ArchiveCase
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : IRequest<Result>
    {
        public required string ParticipantId { get; set; }
        [Description("Reason for Archive")] public ArchiveReason ArchiveReason { get; set; } = ArchiveReason.CaseloadTooHigh;
        [Description("Justification for Archive")] public string? Justification { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants.FindAsync(request.ParticipantId);
            participant!.TransitionTo(EnrolmentStatus.ArchivedStatus, request.ArchiveReason.Name, request.Justification);

            // ReSharper disable once MethodHasAsyncOverload
            return Result.Success();
        }
    }

    public class A_ParticipantMustExistValidator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;
        public A_ParticipantMustExistValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .NotNull()
                .MinimumLength(9)
                .MaximumLength(9)
                .WithMessage("Invalid Participant Id")
                .MustAsync(MustExist)
                .WithMessage("Participant does not exist");
        }
        private async Task<bool> MustExist(string identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Justification)
                .NotEmpty()
                .When(c => c.ArchiveReason.RequiresJustification)
                .WithMessage("You must provide a justification for the selected Archive reason")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Justification"));
        }
    }
}