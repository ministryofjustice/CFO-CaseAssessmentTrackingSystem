using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Participants;

public class Consent : BaseAuditableEntity<int>, ILifetime
{
    private string _participantId;
    private Guid _documentId;
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Consent()
    {
        
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private Consent(string participantId, DateTime consentDate, Guid documentId)
    {
        _participantId = participantId;
        _documentId = documentId;

        // we do not currently require consent to be renewed, so make it the end date.
        Lifetime = new Lifetime(consentDate, DateTime.MaxValue.Date);
        
        AddDomainEvent(new ConsentCreatedDomainEvent(this, _participantId, consentDate));
    }
    
    public Document? Document { get; private set; }

    public static Consent Create(string participantId, DateTime consentDate, Guid documentId) => new(participantId, consentDate, documentId);

    public Lifetime Lifetime { get; }
}
