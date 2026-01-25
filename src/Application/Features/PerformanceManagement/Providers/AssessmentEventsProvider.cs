using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Providers;

public class AssessmentEventsProvider : IPertinentEventProvider
{
    public async Task<IEnumerable<DipEventInformation>> GetEvents(string participantId, IApplicationDbContext context)
    {
        var query = from a in context.ParticipantAssessments
            join u in context.Users on a.CreatedBy equals u.Id
            where a.ParticipantId == participantId
            select new
            {
                u.DisplayName,
                a.Created,
                a.Completed,
                a.IsCompleted
            };

        var results = await query.AsNoTracking().ToArrayAsync();

        return results.Select(a => new DipEventInformation
        {
            Title = "Assessment Created",
            Contents =
            [
                "Assessment added",
                a.IsCompleted ? "Completed" : "Incomplete"
            ],
            ActionedBy = a.DisplayName,
            OccurredOn = a.Created!.Value,
            RecordedOn = a.Created!.Value,
            Icon = AppIcon.Enrolment
        });

    }
}
