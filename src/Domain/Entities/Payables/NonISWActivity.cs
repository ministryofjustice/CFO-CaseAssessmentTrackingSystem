using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Payables;

public class NonISWActivity : Activity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    NonISWActivity()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    NonISWActivity(
        ActivityDefinition definition,
        string participantId,
        Location tookPlaceAtLocation,
        Contract tookPlaceAtContract,
        Location participantCurrentLocation,
        Contract? participantCurrentContract,
        EnrolmentStatus participantStatus,
        string? additionalInformation,
        DateTime completed,
        string tenantId) : base(definition, participantId, tookPlaceAtLocation, tookPlaceAtContract, participantCurrentLocation, participantCurrentContract, participantStatus, additionalInformation, completed, tenantId)
    {
        AddDomainEvent(new NonISWActivityCreatedDomainEvent(this));
    }

    public static NonISWActivity Create(
        ActivityDefinition definition,
        string participantId,
        Location tookPlaceAtLocation,
        Contract tookPlaceAtContract,
        Location participantCurrentLocation,
        Contract? participantCurrentContract,
        EnrolmentStatus participantStatus,
        string? additionalInformation,
        DateTime completed,
        string tenantId)
    {
        NonISWActivity activity = new(
            definition,
            participantId,
            tookPlaceAtLocation,
            tookPlaceAtContract,
            participantCurrentLocation,
            participantCurrentContract,
            participantStatus,
            additionalInformation,
            completed,
            tenantId);

        return activity;
    }
}
