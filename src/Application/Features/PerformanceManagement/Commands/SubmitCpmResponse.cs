using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Commands;

public static class SubmitCpmResponse
{
    [RequestAuthorize(Policy = SecurityPolicies.OutcomeQualityDipVerification)]
    public record Command : IRequest<Result>
    {
        public required UserProfile CurrentUser { get; set; }
        public required string ParticipantId { get; set; }
        public required Guid DipSampleId { get; set; }

        [Description("Comments")]
        public string? Comments { get; set; }

        [Description("Does this review meet compliance")]
        public ComplianceAnswer ComplianceAnswer { get; set; } = ComplianceAnswer.NotAnswered;
    }

    private class Handler(IUnitOfWork unitOfWork, IDateTime dateTime) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var dip = await unitOfWork.DbContext
                .OutcomeQualityDipSampleParticipants
                .FirstAsync(dsp => 
                    dsp.DipSampleId == request.DipSampleId && dsp.ParticipantId == request.ParticipantId, 
                    cancellationToken);

            dip.CpmAnswer(
                isCompliant: request.ComplianceAnswer,
                comments: request.Comments!,
                reviewBy: request.CurrentUser.UserId,
                reviewedOn: dateTime.Now
            );

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            RuleFor(x => x.ParticipantId)
                .NotNull()
                .Length(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

            RuleFor(x => x.DipSampleId)
                .NotEmpty()
                .WithMessage("DipSampleId is required");

            RuleFor(x => x.Comments)
                .NotEmpty()
                .WithMessage("Comments are required")
                .MaximumLength(ValidationConstants.NotesLength)
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Comments"));

            RuleFor(x => x.ComplianceAnswer)
                .Must(x => x.IsAnswer)
                .WithMessage("Compliance must be answered");

            RuleSet(ValidationConstants.RuleSet.MediatR, () => 
            {
                const string message = "Cannot submit CPM review: {0}";

                RuleFor(c => c)
                    .Must((c) => Exist(c.DipSampleId, c.ParticipantId))
                    .WithMessage(string.Format(message, "not found"))
                    .Must((c) => BeInReviewedStatus(c.DipSampleId))
                    .WithMessage(string.Format(message, $"sample must be in '{DipSampleStatus.Reviewed}' status"))
                    .Must((c) => NotHaveFinalisedAnswer(c.DipSampleId, c.ParticipantId))
                    .WithMessage(string.Format(message, "cannot override a finalised answer"));
            });
        }

        private bool Exist(Guid dipSampleId, string participantId) 
            => unitOfWork.DbContext.OutcomeQualityDipSampleParticipants.Any(dsp => dsp.DipSampleId == dipSampleId && dsp.ParticipantId == participantId);
        
        private bool BeInReviewedStatus(Guid dipSampleId)
            => unitOfWork.DbContext.OutcomeQualityDipSamples.Any(ds => ds.Id == dipSampleId && ds.Status == DipSampleStatus.Reviewed);
        
        private bool NotHaveFinalisedAnswer(Guid dipSampleId, string participantId)
        {
            var dsp = unitOfWork.DbContext.OutcomeQualityDipSampleParticipants
                .Where(dsp => dsp.DipSampleId == dipSampleId && dsp.ParticipantId == participantId)
                .Select(dsp => dsp.FinalIsCompliant)
                .First();

            return dsp.IsAnswer is false;
        }
    }
}
