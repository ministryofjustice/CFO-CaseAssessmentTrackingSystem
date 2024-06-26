using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Participants;

public class RightToWork : BaseAuditableEntity<int>, ILifetime
{
    private string _participantId;
    private Guid? _documentId;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private RightToWork()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private RightToWork(string participantId, DateTime validFrom, DateTime validTo, Guid? documentId)
    {
        _participantId = participantId;
        _documentId = documentId;
        Lifetime = new Lifetime(validFrom, validTo);
        AddDomainEvent(new RightToWorkCreatedDomainEvent(this));
    }

    public Document? Document { get; private set; }
    public static RightToWork Create(string participantId, DateTime validFrom, DateTime validTo, Guid documentId) 
        => new(participantId, validFrom, validTo, documentId);

    public Lifetime Lifetime { get; }
}
