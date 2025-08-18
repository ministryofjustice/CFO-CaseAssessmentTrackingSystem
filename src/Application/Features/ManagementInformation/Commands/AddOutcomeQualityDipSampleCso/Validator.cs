using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.ManagementInformation.Commands.AddOutcomeQualityDipSampleCso;

public class Validator : AbstractValidator<Command>
{
    private readonly IUnitOfWork unitOfWork;

    public Validator(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;

        RuleFor(x => x.ParticipantId)
            .NotNull()
            .MaximumLength(9)
            .MinimumLength(9)
            .Matches(ValidationConstants.AlphaNumeric)
            .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

        RuleFor(x => x.DipSampleId)
            .NotEmpty()
            .WithMessage("DipSampleId is required");

        RuleFor(x => x.HasClearParticipantJourney)
            .Must(x => x.IsAnswer)
            .WithMessage("Must be answered");

        RuleFor(x => x.HasClearParticipantJourney)
            .Must(x => x == DipSampleAnswer.NotApplicable == false)
            .WithMessage("Not applicable is not a valid answer for this question");

        RuleFor(x => x.ShowsTaskProgression)
            .Must(x => x.IsAnswer)
            .WithMessage("Must be answered");

        RuleFor(x => x.ShowsTaskProgression)
            .Must(x => x == DipSampleAnswer.NotApplicable == false)
            .WithMessage("Not applicable is not a valid answer for this question");

        RuleFor(x => x.ActivitiesLinkToTasks)
            .Must(x => x == DipSampleAnswer.NotApplicable == false)
            .WithMessage("Not applicable is not a valid answer for this question");

        RuleFor(x => x.ActivitiesLinkToTasks)
            .Must(x => x.IsAnswer)
            .WithMessage("Must be answered");

        RuleFor(x => x.TTGDemonstratesGoodPRIProcess)
            .Must(x => x.IsAnswer)
            .WithMessage("Must be answered");
        
        RuleFor(x => x.SupportsJourney)
            .Must(x => x.IsAnswer)
            .WithMessage("Must be answered");

        RuleFor(x => x.AlignsWithDoS)
            .Must(x => x.IsAnswer)
            .WithMessage("Must be answered");

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
                .Must((c) => BeInAwaitingReviewStatus(c.DipSampleId))
                .WithMessage(string.Format(message, $"sample must be in '{DipSampleStatus.AwaitingReview}' status"))
                .Must((c) => NotHaveCpmOrFinalisedAnswer(c.DipSampleId, c.ParticipantId))
                .WithMessage(string.Format(message, "cannot review a verified/finalised answer"));
        });
    }

    private bool Exist(Guid dipSampleId, string participantId)
        => unitOfWork.DbContext.OutcomeQualityDipSampleParticipants.Any(dsp => dsp.DipSampleId == dipSampleId && dsp.ParticipantId == participantId);

    private bool BeInAwaitingReviewStatus(Guid dipSampleId)
        => unitOfWork.DbContext.OutcomeQualityDipSamples.Any(ds => ds.Id == dipSampleId && ds.Status == DipSampleStatus.AwaitingReview);

    private bool NotHaveCpmOrFinalisedAnswer(Guid dipSampleId, string participantId)
    {
        var answers = unitOfWork.DbContext.OutcomeQualityDipSampleParticipants
            .Where(dsp => dsp.DipSampleId == dipSampleId && dsp.ParticipantId == participantId)
            .Select(dsp => new { dsp.FinalIsCompliant, dsp.CpmIsCompliant })
            .First();

        return answers is { CpmIsCompliant.IsAnswer: false, FinalIsCompliant.IsAnswer: false };
    }
}
