using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.ManagementInformation.Commands;

public static class AddOutcomeQualityDipSampleCpm
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

    class Handler(IUnitOfWork unitOfWork, IDateTime dateTime) : IRequestHandler<Command, Result>
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
        public Validator(IUnitOfWork unitOfWork)
        {
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

            RuleSet(ValidationConstants.RuleSet.MediatR, () => {

                RuleFor(x => x)
                    .MustAsync(async (_, answer, context, canc) => {
                        var dip = await unitOfWork.DbContext.OutcomeQualityDipSampleParticipants
                            .Where(x => x.ParticipantId == answer.ParticipantId && x.DipSampleId == answer.DipSampleId)
                            .AsNoTracking()
                            .Select(x => x.FinalIsCompliant)
                            .FirstOrDefaultAsync(canc);

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

                    }).WithMessage("Cannot submit CPM review: {Reason}");

            });
        }
    }
}
