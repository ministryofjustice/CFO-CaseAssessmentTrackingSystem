using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PRIs.Commands;

public static class AbandonPRI
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]

    public class Command : IRequest<Result>
    {
        [Description("Participant Id")]
        public required string ParticipantId { get; set; }

        [Description("Reason Abandoned")]
        public required PriAbandonReason AbandonReason { get; set; } = PriAbandonReason.Other;

        [Description("Abandoned Justification")]
        public string? AbandonJustification { get; set; }

        [Description("Abandoned By")]
        public required string AbandonedBy { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var pri = await unitOfWork.DbContext.PRIs
                .SingleOrDefaultAsync(p => p.ParticipantId == request.ParticipantId
                && PriStatus.ActiveList.Contains(p.Status), cancellationToken);

            pri!.Abandon(request.AbandonReason, request.AbandonJustification, request.AbandonedBy);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.ParticipantId)
                .Length(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

            RuleFor(c => c.AbandonedBy)
                    .NotNull()
                    .MinimumLength(36);

            RuleFor(c => c.AbandonJustification)
                .NotEmpty()
                .When(c => c.AbandonReason.RequiresJustification)
                .WithMessage("You must provide a justification for the selected Abandoned reason")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Justification"));
        }
    }

    public class A_PRIExists : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public A_PRIExists(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(p => p.ParticipantId)
                .NotNull()
                .Length(9)
                .WithMessage("Invalid Participant Id")
                .Must(Exist)
                .WithMessage("PRI does not exist")
                .Matches(ValidationConstants.AlphaNumeric).WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));
        }

        // Check if the PRI exists in the database
        private bool Exist(string participantId)
            =>  _unitOfWork.DbContext.PRIs.Any(p => p.ParticipantId == participantId);
    }

    public class B_PRIMustNotAlreadyBeCompletedRejected : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public B_PRIMustNotAlreadyBeCompletedRejected(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(p => p.ParticipantId)
                .Must(NotBeCompletedRejected)
                .WithMessage("PRI has already been abandoned/rejected")
                .Matches(ValidationConstants.AlphaNumeric).WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));
        }

        // Check if the participant exists in the database
        private bool NotBeCompletedRejected(string participantId)
            => _unitOfWork.DbContext.PRIs.Any(p => p.ParticipantId == participantId
                    && PriStatus.ActiveList.Contains(p.Status));                        
    }

    public class C_ParticipantMustBeActive : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public C_ParticipantMustBeActive(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .Must(MustNotBeArchived);
        }

        bool MustNotBeArchived(string participantId)
                => _unitOfWork.DbContext.Participants.Any(e => e.Id == participantId && e.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value);
    }

    public class D_ParticipantMustExist : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public D_ParticipantMustExist(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(d => d.ParticipantId)
                .Must(Exist);
        }

        bool Exist(string identifier) => _unitOfWork.DbContext.Participants.Any(e => e.Id == identifier);
    }
}