using Cfo.Cats.Application.Features.QualityAssurance.IntegrationEvents;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordEnrolmentPaymentConsumer(IUnitOfWork unitOfWork) : IHandleMessages<EnrolmentApprovedAtQaIntegrationEvent>
{
    public async Task Handle(EnrolmentApprovedAtQaIntegrationEvent context)
    {
            // get participant information
            var participantInfo = await unitOfWork.DbContext
                .Participants
                .IgnoreAutoIncludes()
                .AsNoTracking()
                .Where(p => p.Id == context.ParticipantId)
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

            var submissionToAuthority = await unitOfWork.DbContext
                .EnrolmentQa1Queue
                .Where(p => p.ParticipantId == context.ParticipantId)
                .MaxAsync(p => p.Created);

            var submissionsToAuthority = await unitOfWork.DbContext
                .EnrolmentQa1Queue
                .Where(p => p.ParticipantId == context.ParticipantId)
                .CountAsync();

            var supportWorker = await unitOfWork.DbContext.EnrolmentPqaQueue
                .Where(q => q.ParticipantId == context.ParticipantId)
                .OrderByDescending(q => q.Created)
                .Select(pqa => new
                {
                    SupportWorkerId = pqa.SupportWorkerId!,
                    pqa.TenantId,
                    SubmissionToPqa = pqa.Created
                })
                .FirstAsync();

            // do we already have a payment?
            var exists = unitOfWork.DbContext.EnrolmentPayments
                .Any(p => p.ParticipantId == context.ParticipantId && p.EligibleForPayment);

            var payment = new EnrolmentPaymentBuilder()
                .WithParticipantId(context.ParticipantId)
                .WithSupportWorker(supportWorker.SupportWorkerId)
                .WithContractId(participantInfo.ContractId)
                .WithConsentAdded(participantInfo.Consent.ConsentAdded!.Value)
                .WithConsentSigned(participantInfo.Consent.ConsentSigned)
                .WithSubmissionToPqa(supportWorker.SubmissionToPqa!.Value)
                .WithSubmissionToAuthority(submissionToAuthority!.Value)
                .WithSubmissionsToAuthority(submissionsToAuthority)
                .WithApproved(context.ApprovalDate.Date)
                .WithLocationId(participantInfo.LocationId)
                .WithLocationType(participantInfo.LocationType)
                .WithTenantId(supportWorker.TenantId)
                .WithReferralRoute(participantInfo.ReferralSource)
                .WithEligibleForPayment(exists == false)
                .WithIneligibilityReason(exists == false ? null : "Already paid")
                .Build();

            unitOfWork.DbContext.EnrolmentPayments.Add(payment);
            await unitOfWork.SaveChangesAsync(CancellationToken.None);

    }
}