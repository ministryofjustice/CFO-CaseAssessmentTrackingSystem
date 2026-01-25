using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Providers.Payments;

public class SupportAndReferralPaymentsEventInformation : IPertinentEventProvider
{
    public async Task<IEnumerable<DipEventInformation>> GetEvents(string participantId, IApplicationDbContext context)
    {
        var query = from p in context.SupportAndReferralPayments
            where p.ParticipantId == participantId
            select new
            {
                p.EligibleForPayment,
                p.SupportType,
                p.IneligibilityReason,
                p.CreatedOn
            };

        var results = await query
            .AsNoTracking()
            .ToArrayAsync();

        return results.Select(r => new DipEventInformation
        {
            Title = $"Support and Referral Payment {(r.EligibleForPayment ? "Paid" : "Rejected")}",
            Colour = r.EligibleForPayment ? AppColour.Success : AppColour.Warning,
            Contents =
            [
                $"{r.SupportType} payment details recorded",
                r.IneligibilityReason ?? string.Empty,
            ],
            ActionedBy = "System",
            OccurredOn = r.CreatedOn,
            RecordedOn = r.CreatedOn,
            Icon = AppIcon.Payment,
        });

    }
}