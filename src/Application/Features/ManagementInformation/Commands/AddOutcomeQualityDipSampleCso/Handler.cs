namespace Cfo.Cats.Application.Features.ManagementInformation.Commands.AddOutcomeQualityDipSampleCso;

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
            linksToTasks: request.ActivitiesLinkToTasks,
            ttgDemonstratesGoodPRIProcess: request.TTGDemonstratesGoodPRIProcess,
            supportsJourney: request.SupportsJourney,
            alignsWithDoS: request.AlignsWithDoS,
            isCompliant: request.ComplianceAnswer,
            comments: request.Comments!,
            reviewBy: request.CurrentUser.UserId,
            reviewedOn: dateTime.Now
        );

        return Result.Success();

    }
}
