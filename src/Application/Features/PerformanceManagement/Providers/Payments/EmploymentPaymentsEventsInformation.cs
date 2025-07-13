using Cfo.Cats.Application.Common.Enums;
using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Providers.Payments;

public class EmploymentPaymentsEventsInformation : IPertinentEventProvider
{
    public async Task<IEnumerable<DipEventInformation>> GetEvents(string participantId, IApplicationDbContext context)
    {
        var query = from p in context.EmploymentPayments
            where p.ParticipantId == participantId
            select new
            {
                p.EligibleForPayment,
                p.IneligibilityReason,
                p.CreatedOn
            };

        var results = await query
            .AsNoTracking()
            .ToArrayAsync();

        return results.Select(r => new DipEventInformation
        {
            Title = $"Employment Payment {(r.EligibleForPayment ? "Paid" : "Rejected")}",
            Colour = r.EligibleForPayment ? AppColour.Success : AppColour.Warning,
            Contents =
            [
                $"Employment payment details recorded",
                r.IneligibilityReason ?? string.Empty,
            ],
            ActionedBy = "System",
            OccurredOn = r.CreatedOn,
            RecordedOn = r.CreatedOn,
            Icon = AppIcon.Payment,
        });

    }
}