using Cfo.Cats.Application.Common.Enums;
using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Providers;

public class ParticipantCreationEventProvider : IPertinentEventProvider
{
    public async Task<IEnumerable<DipEventInformation>> GetEvents(string participantId, IApplicationDbContext context)
    {
        var query = (from p in context.Participants
            join u in context.Users on p.CreatedBy equals u.Id
            where p.Id == participantId
            select new
            {
                p.Created,
                u.DisplayName,
                p.EnrolmentLocation,
                p.DateOfFirstConsent,
                p.EnrolmentLocationJustification,
                Consents = p.Consents.ToArray()
            }).AsNoTracking();

        var participant = await query.FirstAsync();

        string? additionalInformation = participant.EnrolmentLocationJustification;

        if (additionalInformation is not null)
        {
            var locationQuery = (from lh in context.ParticipantLocationHistories
                    join l in context.Locations
                        on lh.LocationId equals l.Id
                    where lh.ParticipantId == participantId
                    orderby lh.From ascending 
                    select l.Name
                ).AsNoTracking();

            var firstLocation = await locationQuery.FirstAsync();
            additionalInformation = $"Participant was registered at {firstLocation}. {additionalInformation}";
        }


        return
        [
            new DipEventInformation
            {
                Contents = [
                    $"The participant enrolled at {participant.EnrolmentLocation.Name}",
                    additionalInformation ?? string.Empty
                ],
                ActionedBy = participant.DisplayName,
                OccurredOn = new DateTime(participant.DateOfFirstConsent!.Value.Year,
                    participant.DateOfFirstConsent.Value.Month, participant.DateOfFirstConsent.Value.Day),
                RecordedOn = participant.Consents.First(c =>
                        DateOnly.FromDateTime(c.Lifetime.StartDate) == participant.DateOfFirstConsent)
                    .Created!.Value,
                Title = "Participant Enrolled",
                Colour = AppColour.Primary,
                Icon = AppIcon.Enrolment,
            }
        ];
    }
}