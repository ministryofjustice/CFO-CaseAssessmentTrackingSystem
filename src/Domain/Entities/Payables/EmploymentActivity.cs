using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Payables;

public class EmploymentActivity : Activity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    EmploymentActivity()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    EmploymentActivity(
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
        AddDomainEvent(new EmploymentActivityCreatedDomainEvent(this));
    }

    /*
    public EmploymentType EmploymentType { get; private set; } // Enum: Full Time, Part Time, Temporary - (requires Vinay's changes!)
    public string EmployerName { get; private set; }
    public string JobTitle { get; private set; }
    public string JobTitleCode { get; private set; } // SOC Code
    public double? Salary { get; private set; } // Nullable, optional
    public DateTime EmploymentCommenced { get; private set; }
    public Document Document { get; private set; } // Uploaded template
    */

    public static EmploymentActivity Create(
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
        EmploymentActivity activity = new(
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
