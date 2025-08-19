using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Mappings;

#pragma warning disable CS8601, CS8602 // Possible null reference assignment, Dereference of a possibly null reference.
public static class ParticipantMappings
{
    public static readonly Expression<Func<Participant, ParticipantPaginationDto>> ToPaginationDto =
        entity => new()
        {
            Id = entity.Id,
            EnrolmentStatus = entity.EnrolmentStatus,
            ConsentStatus = entity.ConsentStatus,
            ParticipantName = $"{entity.FirstName} {entity.LastName}",
            CurrentLocation = entity.CurrentLocation.Name,
            CurrentLocationType = entity.CurrentLocation.LocationType,
            EnrolmentLocation = entity.EnrolmentLocation.Name,
            EnrolmentLocationType = entity.EnrolmentLocation.LocationType,
            Owner = entity.Owner.DisplayName,
            Tenant = entity.Owner.TenantName,
            RiskDue = entity.RiskDue,
            RiskDueReason = entity.RiskDueReason
        };
}
#pragma warning restore CS8601, CS8602 // Possible null reference assignment, Dereference of a possibly null reference.