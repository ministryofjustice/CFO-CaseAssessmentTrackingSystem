using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Commands;

public static class SubmitCsoResponse
{
    [RequestAuthorize(Policy = SecurityPolicies.OutcomeQualityDipReview)]
    public record Command : IRequest<Result>
    {
        public required UserProfile CurrentUser { get; set; }

        public required string ParticipantId { get; set; }
        public required Guid DipSampleId { get; set; }

        /// <summary>
        /// Represents the question A on the CSO sumbission response form
        /// </summary>
        [Description("Does the pathway plan, thematic objectives and tasks meet the participants needs as per the assessment?")]
        public DipSampleAnswer HasClearParticipantJourney { get; set; } = DipSampleAnswer.NotAnswered;

        /// <summary>
        /// Represents the question B on the CSO submission response form
        /// </summary>
        [Description("Is there progression shown under each objective/task?")]
        public DipSampleAnswer ShowsTaskProgression { get; set; } = DipSampleAnswer.NotAnswered;

        /// <summary>
        /// Represents the question C on the CSO submission response form
        /// </summary>
        [Description("Do the activities, including Support Work, Human Citizenship, Community Social, Intervention Services and ETE, link to the tasks?")]
        public DipSampleAnswer SupportsJourney { get; set; } = DipSampleAnswer.NotAnswered;

        /// <summary>
        /// Represents the question D on the CSO submission response form
        /// </summary>
        [Description("Do Human Citizenship, Community Social and Intervention Services link to the DoS and demonstrate good quality outcomes including VFM?")]
        public DipSampleAnswer AlignsWithDoS { get; set; } = DipSampleAnswer.NotAnswered;

        /// <summary>
        /// Represents the question E on the CSO submission response form
        /// </summary>
        [Description("Does the Pre-release support demonstrate focused resettlement activities and immediate practical support that will be required within 4 weeks of release from custody?")]
        public DipSampleAnswer PreReleasePractical { get; set; } = DipSampleAnswer.NotAnswered;

        /// <summary>
        /// Represents the question F on the CSO submission response form
        /// </summary>  
        [Description("Does the TTG Objective include additional tasks that detail links and immediate practical issues to be addressed?")]
        public DipSampleAnswer TTGDemonstratesGoodPRIProcess { get; set; } = DipSampleAnswer.NotAnswered;

        /// <summary>
        /// Represents the question G on the CSO submission response form
        /// </summary>  
        [Description("Has the community Support Worker progressed with TTG objective tasks?")]
        public DipSampleAnswer TTGObjectiveTasks { get; set; } = DipSampleAnswer.NotAnswered;

        [Description("Comments")]
        public string? Comments { get; set; }

        [Description("Does this review meet compliance")]
        public ComplianceAnswer ComplianceAnswer { get; set; } = ComplianceAnswer.NotAnswered;

    }

    internal class Handler(IUnitOfWork unitOfWork, IDateTime dateTime) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var dip = await unitOfWork.DbContext
                .OutcomeQualityDipSampleParticipants
                .Where(x => x.DipSampleId == request.DipSampleId && x.ParticipantId == request.ParticipantId)
                .FirstAsync(cancellationToken);

            dip.CsoAnswer(
                clearJourney: request.HasClearParticipantJourney,
                taskProgression: request.ShowsTaskProgression,
                ttgDemonstratesGoodPRIProcess: request.TTGDemonstratesGoodPRIProcess,
                supportsJourney: request.SupportsJourney,
                alignsWithDoS: request.AlignsWithDoS,
                preReleasePractical: request.PreReleasePractical,
                ttgObjectiveTasks:  request.TTGObjectiveTasks,
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
                .MaximumLength(9)
                .MinimumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

            RuleFor(x => x.DipSampleId)
                .NotEmpty()
                .WithMessage("DipSampleId is required");

            // QUESTION A (Yes/No)
            RuleFor(x => x.HasClearParticipantJourney)
                .Must(x => x.IsAnswer)
                .WithMessage("Must be answered");

            RuleFor(x => x.HasClearParticipantJourney)
                .Must(x => x == DipSampleAnswer.NotApplicable == false)
                .WithMessage("Not applicable is not a valid answer for this question");

            // QUESTION B (Yes/No)
            RuleFor(x => x.ShowsTaskProgression)
                .Must(x => x.IsAnswer)
                .WithMessage("Must be answered");

            RuleFor(x => x.ShowsTaskProgression)
                .Must(x => x == DipSampleAnswer.NotApplicable == false)
                .WithMessage("Not applicable is not a valid answer for this question");

            // QUESTION C (Yes/No)
            RuleFor(x => x.SupportsJourney)
               .Must(x => x.IsAnswer)
               .WithMessage("Must be answered");

            RuleFor(x => x.SupportsJourney)
                .Must(x => x == DipSampleAnswer.NotApplicable == false)
                .WithMessage("Not applicable is not a valid answer for this question");

            // QUESTION D (Yes/No/Na)
            RuleFor(x => x.AlignsWithDoS)
                .Must(x => x.IsAnswer)
                .WithMessage("Must be answered");

            // QUESTION E (Yes/No/Na)
            RuleFor(x => x.PreReleasePractical)
                .Must(x => x.IsAnswer)
                .WithMessage("Must be answered");

            // QUESTION F (Yes/No/Na)
            RuleFor(x => x.TTGDemonstratesGoodPRIProcess)
                .Must(x => x.IsAnswer)
                .WithMessage("Must be answered");

            // QUESTION G (Yes/No/Na)
            RuleFor(x => x.TTGObjectiveTasks)
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

}

