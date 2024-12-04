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
        string description,
        Location location,
        Contract contract,
        string? additionalInformation,
        DateTime completed,
        string completedBy) : base(definition, participantId, description, location, contract, additionalInformation, completed, completedBy)
    {
        AddDomainEvent(new NonISWActivityCreatedDomainEvent(this));
    }

    public static NonISWActivity Create(
        ActivityDefinition definition,
        string participantId,
        string description,
        Location location,
        Contract contract,
        string? additionalInformation,
        DateTime completed,
        string completedBy)
    {
        NonISWActivity activity = new(
            definition,
            participantId,
            description,
            location,
            contract,
            additionalInformation,
            completed,
            completedBy);

        return activity;
    }
}
