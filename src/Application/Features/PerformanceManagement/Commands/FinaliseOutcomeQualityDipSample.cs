using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Commands;

public static class FinaliseOutcomeQualityDipSample
{
    [RequestAuthorize(Policy = SecurityPolicies.OutcomeQualityDipFinalise)]
    public record Command : IRequest<Result>
    {
        public required Guid SampleId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUser) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var sample = await unitOfWork.DbContext.OutcomeQualityDipSamples
                .Include(s => s.Participants)
                .SingleAsync(s => s.Id == request.SampleId, cancellationToken);

            sample.Finalise(currentUser.UserId!);

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

                        if (sample is null)
                        {
                            context.MessageFormatter.AppendArgument("Reason", "not found");
                            return false;
                        }

                        if (sample.Status != DipSampleStatus.Verified)
                        {
                            context.MessageFormatter.AppendArgument("Reason", $"must be in a {DipSampleStatus.Verified} state");
                            return false;
                        }

                        var finalUnanswered = unitOfWork.DbContext.OutcomeQualityDipSampleParticipants
                            .Where(dsp => dsp.DipSampleId == command.SampleId)
                            .Where(dsp => dsp.FinalIsCompliant == ComplianceAnswer.NotAnswered);

                        if (await finalUnanswered.AnyAsync(canc))
                        {
                            context.MessageFormatter.AppendArgument("Reason", $"some responses still need your decision, choose '{ComplianceAnswer.Compliant}' or '{ComplianceAnswer.NotCompliant}' to proceed");
                            return false;
                        }

                        return true;
                    })
                    .WithMessage("Cannot verify: {Reason}");
            });
        }
    }
}

