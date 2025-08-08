using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.ManagementInformation.Commands;

public static class VerifyOutcomeQualityDipSample
{
    [RequestAuthorize(Policy = SecurityPolicies.OutcomeQualityDipVerification)]
    public class Command : IRequest<Result>
    {
        public required Guid SampleId { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUser) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var sample = await unitOfWork.DbContext.OutcomeQualityDipSamples
                .SingleAsync(s => s.Id == request.SampleId, cancellationToken);

            sample.Verify(currentUser.UserId!);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(IUnitOfWork unitOfWork)
        {
            RuleFor(c => c.SampleId)
                .NotNull()
                .WithMessage("SampleId is required");

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {

                RuleFor(c => c)
                    .MustAsync(async (_, command, context, canc) =>
                    {
                        var sample = await unitOfWork.DbContext.OutcomeQualityDipSamples
                            .SingleOrDefaultAsync(s => s.Id == command.SampleId, canc);

                        if(sample is null)
                        {
                            context.MessageFormatter.AppendArgument("Reason", "not found");
                            return false;
                        }

                        if(sample.Status != DipSampleStatus.Reviewed)
                        {
                            context.MessageFormatter.AppendArgument("Reason", $"must be in a {DipSampleStatus.Reviewed} state");
                            return false;
                        }

                        var cpmUnanswered = unitOfWork.DbContext.OutcomeQualityDipSampleParticipants
                            .Where(dsp => dsp.DipSampleId == command.SampleId)
                            .Where(dsp => dsp.CsoIsCompliant == ComplianceAnswer.Unsure)
                            .Where(dsp => dsp.CpmIsCompliant == ComplianceAnswer.NotAnswered);

                        if(await cpmUnanswered.AnyAsync(canc))
                        {
                            context.MessageFormatter.AppendArgument("Reason", $"some '{ComplianceAnswer.Unsure}' responses still need your decision, choose '{ComplianceAnswer.Compliant}' or '{ComplianceAnswer.NotCompliant}' to proceed");
                            return false;
                        }

                        return true;
                    })
                    .WithMessage("Cannot verify: {Reason}");
            });
        }
    }
}