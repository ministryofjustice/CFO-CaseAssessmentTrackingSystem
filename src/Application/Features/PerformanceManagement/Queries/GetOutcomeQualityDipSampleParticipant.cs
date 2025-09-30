using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;
using Cfo.Cats.Application.Features.PerformanceManagement.Providers;
using Cfo.Cats.Application.SecurityConstants;
using System.Runtime.Intrinsics.Arm;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Queries;

public static class GetOutcomeQualityDipSampleParticipant
{
    [RequestAuthorize(Policy = SecurityPolicies.OutcomeQualityDipChecks)]
    public class Query : IRequest<Result<ParticipantDipSampleDto>>
    {
        public required string ParticipantId { get; set; }
        public required Guid SampleId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IEnumerable<IPertinentEventProvider> eventProviders) : IRequestHandler<Query, Result<ParticipantDipSampleDto>>
    {
        public async Task<Result<ParticipantDipSampleDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;

            var participantQuery = 
                from p in db.Participants
                join dsp in db.OutcomeQualityDipSampleParticipants on p.Id equals dsp.ParticipantId
                join dp in db.OutcomeQualityDipSamples on dsp.DipSampleId equals dp.Id
                join c in db.Contracts on dp.ContractId equals c.Id
                where p.Id == request.ParticipantId && dsp.DipSampleId == request.SampleId
                select new ParticipantDipSampleDto
                {
                    ParticipantId = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    DateOfBirth = p.DateOfBirth,
                    Nationality = p.Nationality,
                    SupportWorker = p.Owner!.DisplayName,
                    CurrentLocation = p.CurrentLocation!.Name,
                    EnrolmentLocation = p.EnrolmentLocation!.Name,
                    EnrolmentJustification = p.AssessmentJustification,
                    ConsentDate = p.DateOfFirstConsent!.Value,
                    CsoAnswer = dsp.CsoIsCompliant,
                    CsoComments = dsp.CsoComments,
                    HasClearParticipantJourney = dsp.HasClearParticipantJourney,
                    ShowsTaskProgression = dsp.ShowsTaskProgression,
                    ActivitiesLinkToTasks = dsp.ActivitiesLinkToTasks,
                    TTGDemonstratesGoodPRIProcess = dsp.TTGDemonstratesGoodPRIProcess,
                    SupportsJourney = dsp.SupportsJourney,
                    AlignsWithDoS = dsp.AlignsWithDoS,
                    CpmAnswer = dsp.CpmIsCompliant,
                    CpmComments = dsp.CpmComments,
                    DipSampleStatus = dp.Status,
                    ContractName = c.Description,
                    PeriodFrom = dp.PeriodFrom,
                    FinalAnswer = dsp.FinalIsCompliant,
                    FinalComments = dsp.FinalComments,
                };
            
            var result = await participantQuery.SingleAsync(cancellationToken);

            // now get events
            List<DipEventInformation> events = [];

            foreach (var provider in eventProviders)
            {
                var providerEvents = await provider.GetEvents(request.ParticipantId, unitOfWork.DbContext);
                events.AddRange(providerEvents);
            }
            
            result.PertinentEvents = events.ToArray();

            return result;
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ParticipantId)
                    .MustAsync(async (_, id, context, ct) =>
                    {
                        var participant = await unitOfWork.DbContext.Participants
                            .Where(e => e.Id == id)
                            .Select(e => new { e.DateOfFirstConsent })
                            .AsNoTracking()
                            .FirstOrDefaultAsync(ct);
                        
                        if (participant is null)
                        {
                            context.MessageFormatter.AppendArgument("Reason", "participant not found");
                            return false;
                        }

                        if (participant.DateOfFirstConsent.HasValue == false)
                        {
                            context.MessageFormatter.AppendArgument("Reason", "participant consent has not been granted");
                            return false;
                        }

                        return true;
                    })
                    .WithMessage("Participant is not eligible for DIP sampling: {Reason}");
            });

            //TODO: Add validation that the id belongs to that sample.

        }

       
       
    }
}