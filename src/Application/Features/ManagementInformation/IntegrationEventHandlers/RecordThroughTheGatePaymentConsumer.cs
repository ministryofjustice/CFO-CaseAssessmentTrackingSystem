using Cfo.Cats.Application.Features.PRIs.IntegrationEvents;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using MassTransit;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordThroughTheGatePaymentConsumer(IUnitOfWork unitOfWork)
    : IConsumer<PRIThroughTheGateCompletedIntegrationEvent>
{
    private const string AlreadyPaid = "A Through The Gate payment has already been claimed for this participant";
    private const string MandatoryTasksNotCompleted = "The mandatory tasks have not been completed";
    private const string MandatoryTaskNotCompletedInTime = "The mandatory tasks have not been completed within 4 weeks";
    private const string NotYetApproved = "The enrolment for this participant has not yet been approved";
    private const string NeverInLocation = "The participant has never been in the specified locations";
    private const string BeforeConsent = "This occurred before the consent date";
    
    private const string SupportType = "Through the Gate";

    public async Task Consume(ConsumeContext<PRIThroughTheGateCompletedIntegrationEvent> context)
    {
        var query = from p in unitOfWork.DbContext.PRIs
                    join custodyLocation in unitOfWork.DbContext.Locations on p.CustodyLocationId equals custodyLocation.Id
                    join communityLocation in unitOfWork.DbContext.Locations on p.ExpectedReleaseRegionId equals communityLocation.Id
                    join u in unitOfWork.DbContext.Users on p.CreatedBy equals u.Id
            where p.Id == context.Message.PRIId
                    select new
                    {
                        p.Id,
                        p.ParticipantId,
                        CustodyLocationId = p.CustodyLocationId,
                        CustodyLocationType = custodyLocation.LocationType,
                        CustodyContractId = custodyLocation.Contract!.Id,
                        CommunityLocationId = p.ExpectedReleaseRegionId,
                        CommunityLocationType = communityLocation.LocationType,
                        CommunityContractId = communityLocation.Contract!.Id,
                        u.TenantId,
                        p.CreatedBy,
                        p.MeetingAttendedOn,
                        ReleaseDate = p.ActualReleaseDate,
                        p.ObjectiveId
                    };

        var pri = await query.FirstAsync();

        var tasks = await unitOfWork.DbContext.PathwayPlans
            .AsNoTracking()
            .SelectMany(p => p.Objectives)
            .SelectMany(o => o.Tasks)
            .Where(t => t.ObjectiveId == pri.ObjectiveId)
            .ToArrayAsync();

        

        string? ineligibilityReason = null;


        // All mandatory tasks must be complete
        if (tasks.Where(t => t.IsMandatory).Count(t => t.IsCompleted && t.CompletedStatus == CompletionStatus.Done) != 2)
        {
            ineligibilityReason = MandatoryTasksNotCompleted;
        }

        // 2nd task must be completed within 4 weeks of release
        if (tasks.First(t => t.Index == 2).Completed! > pri.ReleaseDate!.Value.ToDateTime(TimeOnly.MaxValue).AddDays(4 * 7))
        {
            ineligibilityReason = MandatoryTaskNotCompletedInTime;
        }

        // Enrolment must have been approved.
        if (ineligibilityReason == null)
        {
            var history = await unitOfWork.DbContext.ParticipantEnrolmentHistories
                .AsNoTracking()
                .Where(h => h.ParticipantId == pri.ParticipantId)
                .ToListAsync();

            var firstApproval = history.Where(h => h.EnrolmentStatus == EnrolmentStatus.ApprovedStatus)
                .Min(x => x.Created);

            if (firstApproval.HasValue == false)
            {
                ineligibilityReason = NotYetApproved;
            }
        }

        // meeting must be on or after consent date
        if (ineligibilityReason is null)
        {
            var consentDate = await unitOfWork.DbContext
                .Participants
                .AsNoTracking()
                .Where(p => p.Id == pri.ParticipantId)
                .Select(p => p.DateOfFirstConsent)
                .FirstAsync();

            if (consentDate!.Value > pri.MeetingAttendedOn)
            {
                ineligibilityReason = BeforeConsent;
            }
        }

        // Participant must not have had previous TTG payment
        if (ineligibilityReason is null)
        {

            if (await unitOfWork.DbContext
                    .SupportAndReferralPayments
                    .AnyAsync(c => c.ParticipantId == pri.ParticipantId
                                   && c.SupportType == SupportType
                                   && c.EligibleForPayment))
            {
                ineligibilityReason = AlreadyPaid;
            }
        }

        // Participant must be in or have been in region after release date
        if (ineligibilityReason == null)
        {
            var currentLocation = await unitOfWork.DbContext.Participants
                .Where(p => p.Id == pri.ParticipantId)
                .Select(c => new { 
                    ContractId = c.CurrentLocation!.Contract!.Id,
                    LocationType = c.CurrentLocation!.LocationType
                })
                .FirstAsync();

            if (currentLocation.ContractId != pri.CommunityContractId || currentLocation.LocationType.IsCustody)
            {
                // history query
                var historyQuery = from h in unitOfWork.DbContext.ParticipantLocationHistories
                    join l in unitOfWork.DbContext.Locations on h.LocationId equals l.Id
                    where h.ParticipantId == pri.ParticipantId
                    select new
                    {
                        h.From,
                        l.Id
                    };

                var history = await historyQuery.ToListAsync();

                bool beenInCustody = false;
                bool beenInCommunityAfter = false;
                

                foreach (var h in history.Where(h => h.From >
                                                     pri.ReleaseDate!.Value.ToDateTime(TimeOnly.MinValue).AddDays(-90)))
                {
                    if (h.Id == pri.CustodyLocationId)
                    {
                        beenInCustody = true;
                        continue;
                    }

                    if (h.Id == pri.CommunityLocationId && beenInCustody)
                    {
                        beenInCommunityAfter = true;
                        break;
                    }
                }

                if (beenInCommunityAfter == false)
                {
                    ineligibilityReason = NeverInLocation;
                }
            }
        }


        DateTime approved = tasks.FirstOrDefault(t => t.Index == 2 && t.CompletedStatus == CompletionStatus.Done) ==
                            null
            ? DateTime.Now.Date
            : tasks.First(t => t.Index == 2).Completed!.Value;
                

        var payment = new SupportAndReferralBuilder()
            .WithParticipantId(pri.ParticipantId)
            .WithPri(pri.Id)
            .WithApproved(approved)
            .WithLocationId(pri.CustodyLocationId)
            .WithContractId(pri.CommunityContractId)
            .WithSupportType(SupportType)
            .WithTenantId(pri.TenantId)
            .WithLocationType(pri.CommunityLocationType.Name)
            .WithEligibleForPayment(ineligibilityReason == null)
            .WithIneligibilityReason(ineligibilityReason)
            .WithSupportWorker(pri.CreatedBy)
            .Build();

        unitOfWork.DbContext.SupportAndReferralPayments.Add(payment);

        await unitOfWork.SaveChangesAsync();
    }

}