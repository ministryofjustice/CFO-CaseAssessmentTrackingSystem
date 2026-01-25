using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Providers;

public class EnrolmentStatusChangeEventProvider : IPertinentEventProvider
{
    public async Task<IEnumerable<DipEventInformation>> GetEvents(string participantId, IApplicationDbContext context)
    {
        var query = (from eh in context.ParticipantEnrolmentHistories
            join u in context.Users on eh.CreatedBy equals u.Id
            where eh.ParticipantId == participantId
            select new
            {
                eh.EnrolmentStatus,
                eh.AdditionalInformation,
                eh.Reason,
                u.DisplayName,
                eh.Created
            }).AsNoTracking();

        var results = await query.ToArrayAsync();

        return results
            .Where(l => l.EnrolmentStatus == EnrolmentStatus.ApprovedStatus ||
                        l.EnrolmentStatus == EnrolmentStatus.ArchivedStatus)
            .Select(r => new DipEventInformation
            {
                Title = "Major Enrolment Status Change",
                ActionedBy = r.DisplayName,
                Contents = [
                    $"Enrolment Status changed to {r.EnrolmentStatus.Name}",
                    r.Reason is null ? "" : $"{r.Reason} {r.AdditionalInformation}".Trim()
                ],
                OccurredOn = r.Created!.Value,
                RecordedOn = r.Created!.Value,
                Colour = AppColour.Secondary,
                Icon = AppIcon.Enrolment,
            });
    }
}