using Cfo.Cats.Application.Common.Enums;
using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Providers.Payments;

public class ActivityPaymentEventInformation : IPertinentEventProvider
{
    public async Task<IEnumerable<DipEventInformation>> GetEvents(string participantId, IApplicationDbContext context)
    {
        var query = from p in context.ActivityPayments
            where p.ParticipantId == participantId
            select new 
            {
                p.EligibleForPayment,
                p.ActivityType,
                p.ActivityCategory,
                p.IneligibilityReason,
                p.CreatedOn
            };

        var results = await query.AsNoTracking().ToArrayAsync();

        return results.Select(r => new DipEventInformation
        {
            Title = $"Activity Payment {(r.EligibleForPayment ? "Paid" : "Rejected")}",
            Colour = r.EligibleForPayment ? AppColour.Success : AppColour.Warning,
            Contents =
            [
                $"{r.ActivityType} {r.ActivityCategory} payment details recorded",
                r.IneligibilityReason ?? string.Empty,
            ],
            ActionedBy = "System",
            OccurredOn = r.CreatedOn,
            RecordedOn = r.CreatedOn,
            Icon = AppIcon.Payment,
        });

    }
}