using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.ManagementInformation.Commands.AddOutcomeQualityDipSampleCso;

public class Validator : AbstractValidator<Command>
{
    public Validator(IUnitOfWork unitOfWork)
    {
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
        
        RuleFor(x => x.ShowsTaskProgression)
            .Must(x => x.IsAnswer)
            .WithMessage("Must be answered");

        RuleFor(x => x.ActivitiesLinkToTasks)
            .Must(x => x.IsAnswer)
            .WithMessage("Must be answered");

        RuleFor(x => x.TTGDemonstratesGoodPRIProcess)
            .Must(x => x.IsAnswer)
            .WithMessage("Must be answered");

        RuleFor(x => x.TemplatesAlignWithREG)
            .Must(x => x.IsAnswer)
            .WithMessage("Must be answered");
        
        RuleFor(x => x.SupportsJourneyAndAlignsWithDoS)
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
        
        RuleSet(ValidationConstants.RuleSet.MediatR, () => {

            RuleFor(x => x)
                .MustAsync(async (_, answer, context, ct) => {
                    var dip = await unitOfWork.DbContext.OutcomeQualityDipSampleParticipants
                        .Where(x => x.ParticipantId == answer.ParticipantId && x.DipSampleId == answer.DipSampleId)
                        .AsNoTracking()
                        .Select(x => x.FinalIsCompliant)
                        .FirstOrDefaultAsync();

                    if (dip == null)
                    {
                        context.MessageFormatter.AppendArgument("Reason", "not found");
                        return false;
                    }

                    if (dip.IsAnswer)
                    {
                        context.MessageFormatter.AppendArgument("Reason", "Already reviewed");
                        return false;
                    }

                    return true;

                }).WithMessage("Cannot submit CSO review: {Reason}");

        });
        
    }
}
