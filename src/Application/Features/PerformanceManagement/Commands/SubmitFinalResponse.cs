using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Commands;
public static class SubmitFinalResponse
{
    [RequestAuthorize(Policy = SecurityPolicies.OutcomeQualityDipFinalise)]
    public record Command : IRequest<Result>
    {
        [Description("Comments")]
        public string? Comments { get; set; }

        public required string ParticipantId { get; set; }
        public required Guid DipSampleId { get; set; }

        [Description("Does this review meet compliance")]
        public ComplianceAnswer ComplianceAnswer { get; set; } = ComplianceAnswer.NotAnswered;
    }

    public class Handler(IUnitOfWork unitOfWork, IDateTime dateTime, ICurrentUserService currentUserService) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var dip = await unitOfWork.DbContext
                 .OutcomeQualityDipSampleParticipants
                 .FirstAsync(dsp => dsp.DipSampleId == request.DipSampleId && dsp.ParticipantId == request.ParticipantId);

            dip.FinalAnswer(
                    isCompliant: request.ComplianceAnswer,
                    comments: request.Comments!,
                    reviewedBy: currentUserService.UserId!,
                    reviewedOn: dateTime.Now);

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

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c)
                    .MustAsync(async (_, command, context, canc) =>
                    {
                        var query = from dp in unitOfWork.DbContext.OutcomeQualityDipSamples
                                    join dsp in unitOfWork.DbContext.OutcomeQualityDipSampleParticipants
                                    on dp.Id equals dsp.DipSampleId
                                    where dp.Id == command.DipSampleId
                                        && dsp.ParticipantId == command.ParticipantId
                                    select new
                                    {
                                        dp.Status
                                    };

                        var result = await query.FirstOrDefaultAsync(canc);

                        if (result is null)
                        {
                            context.MessageFormatter.AppendArgument("Reason", "not found");
                            return false;
                        }

                        if (result.Status != DipSampleStatus.Verified)
                        {
                            context.MessageFormatter.AppendArgument("Reason", $"must be in a {DipSampleStatus.Verified} state");
                            return false;
                        }

                        return true;
                    })
                    .WithMessage("Cannot finalise: {Reason}");
            });
        }
    }
}
