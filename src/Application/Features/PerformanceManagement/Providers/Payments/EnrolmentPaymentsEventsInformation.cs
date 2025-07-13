using Cfo.Cats.Application.Common.Enums;
using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Providers.Payments;

public class EnrolmentPaymentsEventsInformation : IPertinentEventProvider
{
    public async Task<IEnumerable<DipEventInformation>> GetEvents(string participantId, IApplicationDbContext context)
    {
        var query = from p in context.EnrolmentPayments
            where p.ParticipantId == participantId
            select p;

        var results = await query
            .AsNoTracking()
            .ToArrayAsync();

        return results.Select(r => new DipEventInformation
        {
            Title = $"Support and Referral Payment {(r.EligibleForPayment ? "Paid" : "Rejected")}",
            Colour = r.EligibleForPayment ? AppColour.Success : AppColour.Warning,
            Contents =
            [
                $"Enrolment payment details recorded",
                r.IneligibilityReason ?? string.Empty,
            ],
            ActionedBy = "System",
            OccurredOn = r.CreatedOn,
            RecordedOn = r.CreatedOn,
            Icon = AppIcon.Payment,
        });

    }
}