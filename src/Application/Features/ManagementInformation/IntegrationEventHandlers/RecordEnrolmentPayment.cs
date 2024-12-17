using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using MassTransit;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordEnrolmentPayment(IManagementInformationDbContext miContext, IApplicationDbContext applicationDbContext) : IConsumer<ParticipantTransitionedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ParticipantTransitionedIntegrationEvent> context)
    {
        if (context.Message.To == EnrolmentStatus.ApprovedStatus.Name)
        {

            // get participant information
            var participantInfo = await applicationDbContext
                .Participants
                .IgnoreAutoIncludes()
                .AsNoTracking()
                .Where(p => p.Id == context.Message.ParticipantId)
                .Select(p => new
                {
                    ParticipantId = p.Id,
                    LocationId = p.EnrolmentLocation!.Id,
                    LocationType = p.EnrolmentLocation!.LocationType.Name,
                    p.ReferralSource,
                    ContractId = p.EnrolmentLocation.Contract!.Id,
                    Consent = p.Consents.OrderByDescending(c => c.Created)
                        .Select(x => new
                        {
                            ConsentAdded = x.Created,
                            ConsentSigned = x.Lifetime.StartDate
                        }).First()
                })
                .SingleAsync();

            var submissionToAuthority = await applicationDbContext
                .EnrolmentQa1Queue
                .Where(p => p.ParticipantId == context.Message.ParticipantId)
                .MaxAsync(p => p.Created);

            var submissionsToAuthority = await applicationDbContext
                .EnrolmentQa1Queue
                .Where(p => p.ParticipantId == context.Message.ParticipantId)
                .CountAsync();

            var supportWorker = await applicationDbContext.EnrolmentPqaQueue
                .Where(q => q.ParticipantId == context.Message.ParticipantId)
                .OrderByDescending(q => q.Created)
                .Select(p => new
                {
                    SupportWorkerId = p.CreatedBy!,
                    p.TenantId,
                    SubmissionToPqa = p.Created
                })
                .SingleAsync();

            // do we already have a payment?
            var exists = miContext.EnrolmentPayments
                .Any(p => p.ParticipantId == context.Message.ParticipantId && p.EligibleForPayment);

            var payment = new EnrolmentPaymentBuilder()
                .WithParticipantId(context.Message.ParticipantId)
                .WithSupportWorker(supportWorker.SupportWorkerId)
                .WithContractId(participantInfo.ContractId)
                .WithConsentAdded(participantInfo.Consent.ConsentAdded!.Value)
                .WithConsentSigned(participantInfo.Consent.ConsentSigned)
                .WithSubmissionToPqa(supportWorker.SubmissionToPqa!.Value)
                .WithSubmissionToAuthority(submissionToAuthority!.Value)
                .WithSubmissionsToAuthority(submissionsToAuthority)
                .WithApproved(context.Message.OccuredOn.Date)
                .WithLocationId(participantInfo.LocationId)
                .WithLocationType(participantInfo.LocationType)
                .WithTenantId(supportWorker.TenantId)
                .WithReferralRoute(participantInfo.ReferralSource)
                .WithEligibleForPayment(exists == false)
                .WithIneligibilityReason(exists == false ? null : "Already paid")
                .Build();


            miContext.EnrolmentPayments.Add(payment);
            await miContext.SaveChangesAsync(CancellationToken.None);

        }
    }
}