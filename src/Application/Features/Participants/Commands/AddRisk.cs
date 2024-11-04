
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class AddRisk
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result<Guid>>
    {
        public required string ParticipantId { get; set; }
        [Description("Reason for review")] public RiskReviewReason ReviewReason { get; set; } = RiskReviewReason.ChangeToCircumstances;
        [Description("Justification for reason")] public string? Justification { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            Risk? risk = await unitOfWork.DbContext.Risks
                .Include(x => x.Participant)
                .OrderByDescending(r => r.Created)
                .FirstOrDefaultAsync(r => r.ParticipantId == request.ParticipantId);

            if (risk is null)
            {
                risk = Risk.Create(Guid.NewGuid(), request.ParticipantId, request.ReviewReason, request.Justification);
            }
            else
            {
                risk = Risk.Review(risk, request.ReviewReason, request.Justification);
            }

            await unitOfWork.DbContext.Risks.AddAsync(risk, cancellationToken);
            return Result<Guid>.Success(risk.Id);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ParticipantId)
                .NotNull()
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric);

            RuleFor(c => c.Justification)
                .NotEmpty()
                .When(c => c.ReviewReason.RequiresJustification)
                .WithMessage("You must provide a justification for the selected review reason")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Justification"));
        }
    }

}
