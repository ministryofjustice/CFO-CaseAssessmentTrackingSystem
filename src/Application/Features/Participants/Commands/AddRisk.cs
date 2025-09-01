using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class AddRisk
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result<Guid>>
    {
        public required string ParticipantId { get; set; }
        [Description("Risk location")]
        public LocationDto? Location { get; set; }
        [Description("Reason for review")] public RiskReviewReason ReviewReason { get; set; } = RiskReviewReason.ChangeToCircumstances;
        [Description("Justification for reason")] public string? Justification { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;         
        }

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            // We do nothing async in this method. Await the complete task.
            await Task.CompletedTask;

            Risk? risk = await _unitOfWork.DbContext.Risks
                .Include(x => x.Participant)
                .OrderByDescending(r => r.Created)
                .FirstOrDefaultAsync(r => r.ParticipantId == request.ParticipantId);

            if (risk is null)
            {
                risk = Risk.Create(Guid.CreateVersion7(), request.ParticipantId, request.ReviewReason, request.Location!.Id, request.Justification);
            }
            else
            {
                risk = Risk.Review(risk, request.ReviewReason, request.Justification, request.Location!.Id);
            }

            await _unitOfWork.DbContext.Risks.AddAsync(risk, cancellationToken);
            return Result<Guid>.Success(risk.Id);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.ParticipantId)
                .NotNull()
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));
                      
            RuleFor(x => x.Justification)
                .NotEmpty()
                .When(c => c.ReviewReason.RequiresJustification)
                .WithMessage("You must provide a justification for the selected review reason")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Justification"));
            RuleFor(x => x.Location)
                .NotNull()
                .WithMessage("You must choose a location");

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(x => x.ParticipantId)
                    .MustAsync(Exist)
                    .WithMessage("Participant not found")
                    .MustAsync(MustNotBeArchived)
                    .WithMessage("Participant is archived");
            });
        }
                
        private async Task<bool> MustNotBeArchived(string participantId, CancellationToken cancellationToken)
                => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId && e.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value, cancellationToken);

        private async Task<bool> Exist(string participantId, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId, cancellationToken);
    }
}