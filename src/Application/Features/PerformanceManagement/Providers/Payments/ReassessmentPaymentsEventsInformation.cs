using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Providers.Payments;

public class ReassessmentPaymentsEventsInformation : IPertinentEventProvider
{
    public async Task<IEnumerable<DipEventInformation>> GetEvents(string participantId, IApplicationDbContext context)
    {
        var query = from p in context.ReassessmentPayments
            where p.ParticipantId == participantId
            select new 
            {
                p.EligibleForPayment,
                p.IneligibilityReason,
                p.AssessmentCompleted,
                p.AssessmentCreated
            };

        var results = await query
            .AsNoTracking()
            .ToArrayAsync();

        return results.Select(r => new DipEventInformation
        {
            Title = $"Reassessment Payment {(r.EligibleForPayment ? "Paid" : "Rejected")}",
            Colour = r.EligibleForPayment ? AppColour.Success : AppColour.Warning,
            Contents =
            [
                $"Reassessment payment details recorded",
                r.IneligibilityReason ?? string.Empty,
            ],
            ActionedBy = "System",
            OccurredOn = r.AssessmentCompleted,
            RecordedOn = r.AssessmentCreated,
            Icon = AppIcon.Payment,
        });

    }
}
