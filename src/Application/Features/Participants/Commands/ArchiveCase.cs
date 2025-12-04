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
        [Description("Reason for Archive")] public ArchiveReason ArchiveReason { get; set; } = ArchiveReason.None;
        [Description("Justification for Archive")] public string? Justification { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request.ArchiveReason == ArchiveReason.None)
            {
                return Result.Failure("Invalid archive reason");
            }

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
                .WithMessage("Invalid Participant Id");
            
            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ParticipantId)
                    .MustAsync(MustExist)
                    .WithMessage("Participant does not exist")
                    .MustAsync(MustNotBeArchived)
                    .WithMessage("Participant is archived");
            });
        }

        private async Task<bool> MustExist(string identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);

        private async Task<bool> MustNotBeArchived(string participantId, CancellationToken cancellationToken)
                => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId && e.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value, cancellationToken);
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
      
            RuleFor(c => c.ArchiveReason)
                .Must(x => x != ArchiveReason.None)
                .WithMessage("You must select an archive reason");
        }
    }
}