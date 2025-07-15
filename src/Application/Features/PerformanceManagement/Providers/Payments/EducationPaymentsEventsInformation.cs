using Cfo.Cats.Application.Common.Enums;
using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Providers.Payments;

public class EducationPaymentsEventsInformation : IPertinentEventProvider
{
    public async Task<IEnumerable<DipEventInformation>> GetEvents(string participantId, IApplicationDbContext context)
    {
        var query = from p in context.EducationPayments
            where p.ParticipantId == participantId
            select new
            {
                p.EligibleForPayment,
                p.IneligibilityReason,
                p.CourseTitle,
                p.CourseLevel,
                p.CreatedOn
            };

        var results = await query.AsNoTracking().ToArrayAsync();

        return results.Select(r => new DipEventInformation
        {
            Title = $"Education Payment {(r.EligibleForPayment ? "Paid" : "Rejected")}",
            Colour = r.EligibleForPayment ? AppColour.Success : AppColour.Warning,
            Contents =
            [
                $"Education payment details recorded",
                r.IneligibilityReason ?? string.Empty,
                $"Course: {r.CourseTitle}",
                $"Level: {r.CourseLevel}"
            ],
            ActionedBy = "System",
            OccurredOn = r.CreatedOn,
            RecordedOn = r.CreatedOn,
            Icon = AppIcon.Payment,
        });
    }
}