using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;

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
        string description,
        Location location,
        Contract contract,
        string? additionalInformation,
        DateTime completed,
        string completedBy) : base(definition, participantId, description, location, contract, additionalInformation, completed, completedBy) { }

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
        string description,
        Location location,
        Contract contract,
        string? additionalInformation,
        DateTime completed,
        string completedBy)
    {
        ISWActivity activity = new(
            definition,
            participantId,
            description,
            location,
            contract,
            additionalInformation,
            completed,
            completedBy);

        //activity.AddDomainEvent(new ISWActivityAddedDomainEvent());

        return activity;
    }

}