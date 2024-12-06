using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Payables;

public class ISWActivity : Activity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    ISWActivity()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    ISWActivity(
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
        AddDomainEvent(new ISWActivityCreatedDomainEvent(this));
    }

    /*
    public DateTime WraparoundSupportStart { get; private set; }
    public int HoursPerformedPre { get; private set; }
    public int HoursPerformedDuring { get; private set; }
    public int HoursPerformedPost { get; private set; }
    public DateTime BaselineAchieved { get; private set; }
    */

    public static ISWActivity Create(
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
        ISWActivity activity = new(
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